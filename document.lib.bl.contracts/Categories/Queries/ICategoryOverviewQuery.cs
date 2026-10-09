using document.lib.bl.contracts.Categories.ViewModels;
using document.lib.core;

namespace document.lib.bl.contracts.Categories.Queries;

public interface ICategoryOverviewQuery<in T>
    where T : IUnitOfWork
{
    Task<Result<List<CategoryOverviewModel>>> ExecuteAsync(T uow, CategoryOverviewQueryParameters parameters);
}

public record CategoryOverviewQueryParameters(int? CategoryId = null);
