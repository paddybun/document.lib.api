using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface ICategoryRepository
{
    DocLibCategory GetCategoryById(string id);
    Task<DocLibCategory> CreateCategoryAsync(string category);
    DocLibCategory GetCategoryByName(string categoryName);
}