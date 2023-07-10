using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface ICategoryRepository
{
    Task<DocLibCategory> GetCategoryByIdAsync(string id);
    Task<DocLibCategory> CreateCategoryAsync(DocLibCategory category);
    Task<DocLibCategory> GetCategoryByNameAsync(string categoryName);
}