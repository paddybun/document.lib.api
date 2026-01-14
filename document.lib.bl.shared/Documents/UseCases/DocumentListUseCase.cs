using document.lib.bl.contracts.Documents.Queries;
using document.lib.bl.contracts.Documents.UseCases;
using document.lib.bl.contracts.Documents.ViewModels;
using document.lib.core;
using document.lib.core.Models;
using document.lib.data.entities;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Documents.UseCases;

public class DocumentListUseCase(ILogger<DocumentListUseCase> logger, IDocumentOverviewQuery<UnitOfWork> query)
    : IDocumentListUseCase<UnitOfWork>
{
    public async Task<Result<FilteredResult<DocumentOverviewModel>>> ExecuteAsync(UnitOfWork model,
        OverviewRequestParameters parameters)
    {
        try
        {
            logger.LogInformation("Executing document list query");
            var docs = await query.ExecuteAsync(model, parameters);

            var result = new FilteredResult<DocumentOverviewModel>
            {
                Count = docs.Value!.Count,
                FilteredList = docs.Value!.FilteredList.Select(Map).ToList()
            };

            return Result<FilteredResult<DocumentOverviewModel>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occured");
            return Result<FilteredResult<DocumentOverviewModel>>.Failure(ex.Message);
        }
    }

    private DocumentOverviewModel Map(Document doc)
    {
        return new DocumentOverviewModel
        {
            Id = doc.Id,
            DisplayName = doc.DisplayName ?? string.Empty,
            Unsorted = doc.Unsorted,
            Filename = doc.OriginalFileName,
            Folder = doc.Register.Folder?.DisplayName ?? string.Empty,
            Register = doc.Register.DisplayName ?? string.Empty
        };
    }
}