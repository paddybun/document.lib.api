using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryModel> GetCategoryAsync(string name)
    {
        return await _categoryRepository.GetCategoryAsync(new CategoryQueryParameters(name: name));
    }

    public async Task<List<CategoryModel>> GetAllAsync()
    {
        return await _categoryRepository.GetCategoriesAsync();
    }

    public async Task<CategoryModel> CreateOrGetCategoryAsync(string category)
    {
        var categoryEntity = await _categoryRepository.GetCategoryAsync(new CategoryQueryParameters(name: category));
        var model = new CategoryModel { Name = category };

        if (categoryEntity == null)
        {
            return await _categoryRepository.CreateCategoryAsync(model);
        }

        return categoryEntity;
    }

    public async Task<CategoryModel> SaveAsync(CategoryModel category, bool createNew = false)
    {
        if (!createNew) 
            return await _categoryRepository.UpdateCategoryAsync(category);
        
        category.DisplayName = string.IsNullOrWhiteSpace(category.DisplayName) ? category.Name : category.DisplayName;
        return await _categoryRepository.CreateCategoryAsync(category);
    }
}