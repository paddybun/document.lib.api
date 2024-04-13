using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface ICategoryRepository
{
    Task<CategoryModel?> GetCategoryAsync(CategoryModel categoryModel);
    Task<List<CategoryModel>> GetCategoriesAsync();
    Task<CategoryModel> CreateCategoryAsync(CategoryModel category);
    Task<CategoryModel> UpdateCategoryAsync(CategoryModel category);
}