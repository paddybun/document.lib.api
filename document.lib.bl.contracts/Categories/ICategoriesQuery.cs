using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Categories;

public interface ICategoriesQuery<in T>
    where T : IUnitOfWork
{
    Task<Result<List<Category>>> ExecuteAsync(T uow, CategoriesQueryParameters parameters);
}

public class CategoriesQueryParameters
{
    public int? Skip { get; set; }
    public int? Take { get; set; }
}