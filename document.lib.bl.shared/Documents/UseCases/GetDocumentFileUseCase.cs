using document.lib.bl.contracts.Documents.Queries;
using document.lib.bl.contracts.Documents.UseCases;
using document.lib.bl.contracts.Documents.ViewModels;
using document.lib.bl.contracts.Upload.Commands;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Documents.UseCases;

public class GetDocumentFileUseCase(
    ILogger<GetDocumentFileUseCase> logger,
    IDocumentQuery<UnitOfWork> documentQuery,
    IDownloadBlobCommand downloadBlobCommand) : IGetDocumentFileUseCase<UnitOfWork>
{
    public async Task<Result<DocumentFileModel>> ExecuteAsync(UnitOfWork uow, GetDocumentFileUseCaseParameters parameters)
    {
        try
        {
            var result = await documentQuery.ExecuteAsync(uow, parameters.DocumentId);
            if (result.HasError) return Result<DocumentFileModel>.Failure(result.Message);
            if (result.HasWarning) return Result<DocumentFileModel>.Warning(result.Message);

            var doc = result.Value!;
            var stream = await downloadBlobCommand.ExecuteAsync(doc.BlobLocation);
            if (stream == null) return Result<DocumentFileModel>.Warning("Document file not found");

            return Result<DocumentFileModel>.Success(new DocumentFileModel
            {
                Content = stream,
                FileName = doc.OriginalFileName
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting file for document {id}", parameters.DocumentId);
            return Result<DocumentFileModel>.Failure(ex.Message, ex);
        }
    }
}
