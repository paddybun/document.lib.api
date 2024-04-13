using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using document.lib.shared.Models.QueryDtos;

namespace document.lib.shared.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<CategoryModel?> GetCategoryAsync(string name)
    {
        return await categoryRepository.GetCategoryAsync(new CategoryQueryParameters(name: name));
    }

    public async Task<List<CategoryModel>> GetAllAsync()
    {
        return await categoryRepository.GetCategoriesAsync();
    }

    public async Task<CategoryModel> CreateOrGetCategoryAsync(string category)
    {
        var categoryEntity = await categoryRepository.GetCategoryAsync(new CategoryQueryParameters(name: category));
        var model = new CategoryModel { Name = category };

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