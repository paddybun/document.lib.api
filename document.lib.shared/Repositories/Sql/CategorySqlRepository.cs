using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class CategorySqlRepository(DocumentLibContext context) : ICategoryRepository<EfCategory>
{
    public Task<EfCategory?> GetCategoryByIdAsync(int id)
    {
        var efCategory = context
            .Categories
            .SingleOrDefaultAsync(x => x.Id == id);
        return efCategory;
    }

    public Task<EfCategory?> GetCategoryByNameAsync(string name)
    {
        var efCategory = context
            .Categories
            .SingleOrDefaultAsync(x => x.Name == name);
        return efCategory;
    }

    public async Task<List<EfCategory>> GetCategoriesAsync()
    {
        var categories = await context.Categories.ToListAsync();
        return categories; //.Select(Map).ToList();
    }

    public async Task<EfCategory> CreateCategoryAsync(string name, string? description = null, string? displayName = null)
    {
        var efCategory = new EfCategory
        {
            Name = name,
            Description = description,
            DisplayName = displayName
        };
        await context.Categories.AddAsync(efCategory);
        await context.SaveChangesAsync();
        return efCategory;
    }

    public async Task<EfCategory> UpdateCategoryAsync(EfCategory category)
    {
        context.Update(category);
        await context.SaveChangesAsync();
        return category;
    }
}