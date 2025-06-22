using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Categories;

public interface ICategoryQuery
{
    Task<Result<Category>> ExecuteAsync(string categoryName);
}