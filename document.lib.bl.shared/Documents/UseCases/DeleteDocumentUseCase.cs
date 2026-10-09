using document.lib.bl.contracts.Documents.Commands;
using document.lib.bl.contracts.Documents.Queries;
using document.lib.bl.contracts.Documents.UseCases;
using document.lib.bl.contracts.Upload.Commands;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Documents.UseCases;

public class DeleteDocumentUseCase(
    ILogger<DeleteDocumentUseCase> logger,
    IDocumentQuery<UnitOfWork> documentQuery,
    IDeleteDocumentCommand<UnitOfWork> deleteDocumentCommand,
    ICopyBlobCommand copyBlobCommand,
    IDeleteBlobCommand deleteBlobCommand) : IDeleteDocumentUseCase<UnitOfWork>
{
    private const string DeletedBlobFolder = "deleted";

    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, DeleteDocumentUseCaseParameters parameters)
    {
        string? deletedBlob = null;
        try
        {
            logger.LogInformation("Deleting document {id}", parameters.DocumentId);

            var current = await documentQuery.ExecuteAsync(uow, parameters.DocumentId);
            if (current.HasError) return Result<int>.Failure(current.Message);
            if (current.HasWarning) return Result<int>.Warning(current.Message);
            var doc = current.Value!;

            // Blobs have no real folders; the prefix is created with the first blob
            var target = $"{DeletedBlobFolder}/{doc.PhysicalName}";
            if (!await copyBlobCommand.ExecuteAsync(doc.BlobLocation, target))
                return Result<int>.Failure("Could not copy document blob to the deleted folder");
            deletedBlob = target;

            await uow.BeginTransactionAsync();
            var result = await deleteDocumentCommand.ExecuteAsync(uow, new DeleteDocumentCommandParameters(doc.Id));
            if (!result.IsSuccess)
            {
                await Abort(uow, deletedBlob);
                return result;
            }

            await uow.CommitAsync();
            deletedBlob = null;

            try
            {
                await deleteBlobCommand.ExecuteAsync(doc.BlobLocation);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not delete old blob {blob}", doc.BlobLocation);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting document {id}", parameters.DocumentId);
            await Abort(uow, deletedBlob);
            return Result<int>.Failure(ex.Message, ex);
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
