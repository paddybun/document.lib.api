using document.lib.shared.Models.Models;
using document.lib.shared.Models.QueryDtos;

namespace document.lib.shared.Interfaces;

public interface ICategoryRepository
{
    Task<CategoryModel?> GetCategoryAsync(CategoryQueryParameters queryParameters);
    Task<List<CategoryModel>> GetCategoriesAsync();
    Task<CategoryModel> CreateCategoryAsync(CategoryModel category);
    Task<CategoryModel> UpdateCategoryAsync(CategoryModel category);
}