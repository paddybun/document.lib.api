using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Categories;

public interface ICategoryQuery<in T>
    where T : IUnitOfWork
{
    Task<Result<Category>> ExecuteAsync(T uow, CategoryQueryParameters parameters);
}

public record CategoryQueryParameters(string CategoryName);