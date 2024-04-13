using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class CategorySqlRepository(DocumentLibContext context) : ICategoryRepository
{
    public async Task<CategoryModel?> GetCategoryAsync(CategoryModel categoryModel)
    {
        if (categoryModel == null) throw new ArgumentNullException(nameof(categoryModel));

        if (string.IsNullOrWhiteSpace(categoryModel.Id) && string.IsNullOrWhiteSpace(categoryModel.Name))
            throw new InvalidParameterException(categoryModel.GetType());

        EfCategory? efCategory;
        if (!string.IsNullOrWhiteSpace(categoryModel.Id))
        {
            var id = int.Parse(categoryModel.Id);
            efCategory = await context.Categories.SingleOrDefaultAsync(x => x.Id == id);
        }
        else
        {
            efCategory = await context.Categories.SingleOrDefaultAsync(x => x.Name == categoryModel.Name);
        }

        return efCategory == null ? null : Map(efCategory);
    }

    public async Task<List<CategoryModel>> GetCategoriesAsync()
    {
        var categories = await context.Categories.ToListAsync();
        return categories.Select(Map).ToList();
    }

    public async Task<CategoryModel> CreateCategoryAsync(CategoryModel category)
    {
        var efCategory = new EfCategory
        {
            Name = category.Name,
            Description = category.Description,
            DisplayName = category.DisplayName
        };
        await context.Categories.AddAsync(efCategory);
        await context.SaveChangesAsync();
        return Map(efCategory);
    }

    public async Task<CategoryModel> UpdateCategoryAsync(CategoryModel category)
    {
        await context
            .Categories
            .Where(x => x.Id == int.Parse(category.Id))
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.DisplayName, category.DisplayName)
                .SetProperty(p => p.Description, category.Description));
        return category;
    }

    // TODO: Refactor to corresponding mapper
    private static CategoryModel Map(EfCategory efCategory)
    {
        return new CategoryModel
        {
            Id = efCategory.Id.ToString(),
            Name = efCategory.Name,
            Description = efCategory.Description,
            DateCreated = efCategory.DateCreated,
            DateModified = efCategory.DateModified,
            DisplayName = efCategory.DisplayName
        };
    }
}