using document.lib.ef.Entities;
using document.lib.shared.Enums;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Data;

namespace document.lib.shared.Services;

public class CategorySqlService(ICategoryRepository<EfCategory> categoryRepository) : ICategoryService
{
    public DatabaseProvider DatabaseProvider => DatabaseProvider.Sql;

    public async Task<CategoryModel?> GetCategoryAsync(string name)
    {
        var category = await categoryRepository.GetCategoryByNameAsync(name);
        return category == null ? null : Map(category);
    }

    public async Task<List<CategoryModel>> GetAllAsync()
    {
        var category = await categoryRepository.GetCategoriesAsync();
        return category.Select(Map).ToList();
    }

    public async Task<CategoryModel> CreateOrGetCategoryAsync(string name)
    {
        var categoryEntity = 
            await categoryRepository.GetCategoryByNameAsync(name) ?? 
            await categoryRepository.CreateCategoryAsync(name);

        return Map(categoryEntity);
    }

    public async Task<CategoryModel?> UpdateAsync(CategoryModel category)
    {
        var categoryToUpdate = await categoryRepository.GetCategoryByIdAsync((int)category.Id!);
        if (categoryToUpdate == null) return null;
        
        categoryToUpdate.Description = category.Description;
        categoryToUpdate.DisplayName = category.DisplayName;

        categoryToUpdate = await categoryRepository.UpdateCategoryAsync(categoryToUpdate);
        return Map(categoryToUpdate);
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