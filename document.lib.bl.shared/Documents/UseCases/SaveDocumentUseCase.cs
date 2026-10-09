using document.lib.bl.contracts.DocumentHandling.UseCases;
using document.lib.bl.contracts.Documents.Commands;
using document.lib.bl.contracts.Documents.Queries;
using document.lib.bl.contracts.Documents.UseCases;
using document.lib.bl.contracts.Folders.Queries;
using document.lib.bl.contracts.Upload.Commands;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Documents.UseCases;

public class SaveDocumentUseCase(
    ILogger<SaveDocumentUseCase> logger,
    IDocumentQuery<UnitOfWork> documentQuery,
    IActiveFolderQuery<UnitOfWork> activeFolderQuery,
    IGetRegisterUseCase<UnitOfWork> getRegisterUseCase,
    IUpdateDocumentCommand<UnitOfWork> updateDocumentCommand,
    IMoveDocumentCommand<UnitOfWork> moveDocumentCommand,
    ICopyBlobCommand copyBlobCommand,
    IDeleteBlobCommand deleteBlobCommand) : ISaveDocumentUseCase<UnitOfWork>
{
    public async Task<Result<Document>> ExecuteAsync(UnitOfWork uow, SaveDocumentUseCaseParameters parameters)
    {
        string? copiedBlob = null;
        try
        {
            logger.LogInformation("Saving document {id}", parameters.DocumentId);
            var model = parameters.Document;

            if (string.IsNullOrWhiteSpace(model.DisplayName))
                return Result<Document>.Warning("Display name is required");

            var current = await documentQuery.ExecuteAsync(uow, parameters.DocumentId);
            if (!current.HasData) return current;
            var doc = current.Value!;

            string? oldBlob = null;
            Register? targetRegister = null;
            string? newBlob = null;

            if (doc.Unsorted)
            {
                var folderResult = await activeFolderQuery.ExecuteAsync(uow);
                if (!folderResult.HasData) return Result<Document>.Warning(folderResult.Message);
                var folder = folderResult.Value!;

                var registerResult = await getRegisterUseCase.ExecuteAsync(uow, new(folder.Id));
                if (!registerResult.HasData)
                {
                    await uow.RollbackTransactionAsync();
                    return Result<Document>.Warning(registerResult.Message);
                }

                targetRegister = registerResult.Value!;
                oldBlob = doc.BlobLocation;
                newBlob = $"{folder.Name}/{targetRegister.Name}/{doc.PhysicalName}";

                if (!await copyBlobCommand.ExecuteAsync(oldBlob, newBlob))
                {
                    await uow.RollbackTransactionAsync();
                    return Result<Document>.Failure("Could not copy document blob");
                }
                copiedBlob = newBlob;
            }

            await uow.BeginTransactionAsync();

            var update = await updateDocumentCommand.ExecuteAsync(uow,
                new UpdateDocumentCommandParameters(doc.Id, model.DisplayName, model.DateOfDocument, model.CategoryId, model.Tags));
            if (!update.IsSuccess)
            {
                await Abort(uow, copiedBlob);
                return Result<Document>.Warning(update.Message);
            }

            if (targetRegister != null)
            {
                var move = await moveDocumentCommand.ExecuteAsync(uow,
                    new MoveDocumentCommandParameters(doc.Id, targetRegister.Id, newBlob!));
                if (!move.IsSuccess)
                {
                    await Abort(uow, copiedBlob);
                    return Result<Document>.Warning(move.Message);
                }
            }

            await uow.CommitAsync();
            copiedBlob = null;

            if (oldBlob != null)
            {
                try
                {
                    await deleteBlobCommand.ExecuteAsync(oldBlob);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Could not delete old blob {blob}", oldBlob);
                }
            }

            return await documentQuery.ExecuteAsync(uow, doc.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving document {id}", parameters.DocumentId);
            await Abort(uow, copiedBlob);
            return Result<Document>.Failure(ex.Message, ex);
        }
    }

    private async Task Abort(UnitOfWork uow, string? copiedBlob)
    {
        await uow.RollbackTransactionAsync();
        if (copiedBlob == null) return;

        try
        {
            await deleteBlobCommand.ExecuteAsync(copiedBlob);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not clean up copied blob {blob}", copiedBlob);
        }
    }
}
