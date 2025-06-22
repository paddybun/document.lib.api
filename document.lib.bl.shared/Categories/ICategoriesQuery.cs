using document.lib.data.entities;

namespace document.lib.bl.shared.Categories;

public interface ICategoriesQuery
{
    Task<List<Category>> ExecuteAsync(int? skip = null, int? take = null);
}