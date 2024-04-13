using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;

namespace document.lib.shared.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<CategoryModel?> GetCategoryAsync(string name)
    {
        var categoryModel = new CategoryModel { Name = name };
        return await categoryRepository.GetCategoryAsync(categoryModel);
    }

    public async Task<List<CategoryModel>> GetAllAsync()
    {
        return await categoryRepository.GetCategoriesAsync();
    }

    public async Task<CategoryModel> CreateOrGetCategoryAsync(string category)
    {
        var model = new CategoryModel { Name = category };
        var categoryEntity = await categoryRepository.GetCategoryAsync(model);

        if (categoryEntity == null)
        {
            return await categoryRepository.CreateCategoryAsync(model);
        }

        return categoryEntity;
    }

    public async Task<CategoryModel> SaveAsync(CategoryModel category, bool createNew = false)
    {
        if (!createNew) 
            return await categoryRepository.UpdateCategoryAsync(category);
        
        category.DisplayName = string.IsNullOrWhiteSpace(category.DisplayName) ? category.Name : category.DisplayName;
        return await categoryRepository.CreateCategoryAsync(category);
    }
}