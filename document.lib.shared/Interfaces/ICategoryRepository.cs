using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Interfaces;

public interface ICategoryRepository
{
    Task<CategoryModel?> GetCategoryAsync(CategoryQueryParameters queryParameters);
    Task<List<CategoryModel>> GetCategoriesAsync();
    Task<CategoryModel> CreateCategoryAsync(CategoryModel category);
    Task<CategoryModel> UpdateCategoryAsync(CategoryModel category);
}