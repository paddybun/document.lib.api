using document.lib.bl.contracts.Documents.ViewModels;
using document.lib.core;
using document.lib.core.Models;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Documents.Queries;

public interface IDocumentOverviewQuery<in T> where T : IUnitOfWork
{
    Task<Result<FilteredResult<Document>>> ExecuteAsync(T uow, OverviewRequestParameters parameters);
}