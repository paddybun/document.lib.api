using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;

namespace document.lib.shared.Services;

public class CategorySqlService(ICategoryRepository<EfCategory> categoryRepository) : ICategoryService
{
    public async Task<ITypedServiceResult<CategoryModel>> GetCategoryAsync(int id)
    {
        var category = await categoryRepository.GetCategoryAsync(id);
        return category != null
            ? ServiceResult.Ok(Map(category))
            : ServiceResult.DefaultError<CategoryModel>();
    }

    public async Task<ITypedServiceResult<CategoryModel>> GetCategoryAsync(string name)
    {
        var category = await categoryRepository.GetCategoryAsync(name);
        return category != null
            ? ServiceResult.Ok(Map(category))
            : ServiceResult.DefaultError<CategoryModel>();
    }

    public async Task<ITypedServiceResult<PagedResult<CategoryModel>>> GetCategoriesPagedAsync(int page, int pageSize)
    {
        var categories = await categoryRepository.GetCategoriesPagedAsync(page, pageSize);
        var mappedCategories = categories.Results.Select(Map).ToList();
        return ServiceResult.Ok(new PagedResult<CategoryModel>(mappedCategories, categories.Total));
    }

    public async Task<ITypedServiceResult<CategoryModel>> CreateCategoryAsync(CategoryModel model)
    {
        var name = Guid.NewGuid().ToString();
        var category = new EfCategory
        {
            Name = name,
            DisplayName = model.DisplayName,
            Description = model.Description
        };
        category = await categoryRepository.CreateCategoryAsync(category);
        return ServiceResult.Ok(Map(category));
    }

    public async Task<ITypedServiceResult<CategoryModel>> UpdateCategory(CategoryModel model)
    {
        var category = await categoryRepository.GetCategoryAsync((int)model.Id!);
        if (category == null)
        {
            return ServiceResult.DefaultError<CategoryModel>();
        }
        
        category.DisplayName = model.DisplayName;
        category.Description = model.Description;
        await categoryRepository.SaveAsync();
        
        return ServiceResult.Ok(Map(category));
    }

    private static CategoryModel Map(EfCategory efCategory)
    {
        return new CategoryModel
        {
            Id = efCategory.Id,
            Name = efCategory.Name,
            Description = efCategory.Description,
            DateCreated = efCategory.DateCreated,
            DateModified = efCategory.DateModified,
            DisplayName = efCategory.DisplayName
        };
    }
}