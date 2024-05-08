using document.lib.shared.Enums;
using document.lib.shared.Models.Data;

namespace document.lib.shared.Interfaces;

public interface ICategoryService
{
    // Task<CategoryModel?> GetCategoryAsync(string name);
    // Task<List<CategoryModel>> GetAllAsync();
    // Task<CategoryModel> CreateOrGetCategoryAsync(string category);
    // Task<CategoryModel> SaveAsync(CategoryModel category, bool createNew = false);
    public DatabaseProvider DatabaseProvider { get; }
    Task<CategoryModel?> GetCategoryAsync(string name);
    Task<List<CategoryModel>> GetAllAsync();
    Task<CategoryModel> CreateOrGetCategoryAsync(string name);
    Task<CategoryModel?> UpdateAsync(CategoryModel category);
}