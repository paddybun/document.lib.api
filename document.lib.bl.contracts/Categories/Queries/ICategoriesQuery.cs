using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Categories.Queries;

public interface ICategoriesQuery<in T>
    where T : IUnitOfWork
{
    Task<Result<List<Category>>> ExecuteAsync(T uow, CategoriesQueryParameters parameters);
}

public record CategoriesQueryParameters(int? Skip, int? Take);