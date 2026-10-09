using document.lib.bl.contracts.Tags.ViewModels;
using document.lib.core;

namespace document.lib.bl.contracts.Tags.Queries;

public interface ITagOverviewQuery<in T>
    where T : IUnitOfWork
{
    /// <summary>Returns the tags which are assigned to at least one document.</summary>
    Task<Result<List<TagOverviewModel>>> ExecuteAsync(T uow);
}
