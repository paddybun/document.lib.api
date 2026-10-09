using document.lib.bl.contracts.Documents.Queries;
using document.lib.bl.contracts.Documents.UseCases;
using document.lib.bl.contracts.Documents.ViewModels;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Documents.UseCases;

public class GetDocumentUseCase(ILogger<GetDocumentUseCase> logger, IDocumentQuery<UnitOfWork> query)
    : IGetDocumentUseCase<UnitOfWork>
{
    public async Task<Result<DocumentEditModel>> ExecuteAsync(UnitOfWork uow, GetDocumentUseCaseParameters parameters)
    {
        try
        {
            var result = await query.ExecuteAsync(uow, parameters.DocumentId);
            if (result.HasError) return Result<DocumentEditModel>.Failure(result.Message);
            if (result.HasWarning) return Result<DocumentEditModel>.Warning(result.Message);

            var doc = result.Value!;
            return Result<DocumentEditModel>.Success(new DocumentEditModel
            {
                Id = doc.Id,
                DisplayName = doc.DisplayName,
                CategoryId = doc.Category.Name == Constants.UncategorizedName ? null : doc.CategoryId,
                TagIds = doc.Tags.Select(x => x.Tag.Id).ToList(),
                Filename = doc.OriginalFileName,
                Unsorted = doc.Unsorted,
                Folder = doc.Register.Folder?.DisplayName ?? string.Empty,
                Register = doc.Register.DisplayName ?? string.Empty,
                Company = doc.Company,
                Description = doc.Description,
                DateOfDocument = doc.DateOfDocument,
                UploadDate = doc.UploadDate,
                DateModified = doc.DateModified,
                Digital = doc.Digital,
                FilesizeInBytes = doc.FilesizeInBytes
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting document {id}", parameters.DocumentId);
            return Result<DocumentEditModel>.Failure(ex.Message, ex);
        }
    }
}
