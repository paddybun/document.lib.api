using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;
using document.lib.shared.TableEntities;

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
}