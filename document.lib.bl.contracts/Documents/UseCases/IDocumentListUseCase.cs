using document.lib.bl.contracts.Documents.ViewModels;
using document.lib.core;
using document.lib.core.Models;

namespace document.lib.bl.contracts.Documents.UseCases;

public interface IDocumentListUseCase<in T> where T : IUnitOfWork
{
    Task<Result<FilteredResult<DocumentOverviewModel>>> ExecuteAsync(T model, OverviewRequestParameters parameters);
}