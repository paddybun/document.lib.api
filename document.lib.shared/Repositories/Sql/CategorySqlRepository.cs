using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

// Scoped injection
public sealed class CategorySqlRepository(DocumentLibContext context) : ICategoryRepository
{
    public async Task<CategoryModel?> GetCategoryAsync(CategoryQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (!queryParameters.IsValid()) throw new InvalidQueryParameterException(queryParameters.GetType());

        EfCategory? efCategory;
        if (queryParameters.Id.HasValue)
        {
            efCategory = await context.Categories.SingleOrDefaultAsync(x => x.Id == queryParameters.Id.Value);
        }
        else
        {
            efCategory = await context.Categories.SingleOrDefaultAsync(x => x.Name == queryParameters.Name);
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