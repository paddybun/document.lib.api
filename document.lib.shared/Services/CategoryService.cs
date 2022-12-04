using document.lib.shared.Interfaces;
using document.lib.shared.TableEntities;

namespace document.lib.shared.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<DocLibCategory> CreateOrGetCategoryAsync(string category)
    {
        var categoryEntity = _categoryRepository.GetCategoryByName(category);
        if (categoryEntity == null)
        {
            return await _categoryRepository.CreateCategoryAsync(category);
        }

        return categoryEntity;
    }
}