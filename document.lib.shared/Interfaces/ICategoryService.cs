using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface ICategoryService
{
    Task<CategoryModel?> GetCategoryAsync(string name);
    Task<List<CategoryModel>> GetAllAsync();
    Task<CategoryModel> CreateOrGetCategoryAsync(string category);
    Task<CategoryModel> SaveAsync(CategoryModel category, bool createNew = false);
}