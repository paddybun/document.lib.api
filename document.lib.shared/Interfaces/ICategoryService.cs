using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Interfaces;

public interface ICategoryService
{
    Task<CategoryModel> GetCategoryAsync(string name);

    Task<CategoryModel> CreateOrGetCategoryAsync(string category);
}