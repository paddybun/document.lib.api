using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class CategorySqlRepository(DocumentLibContext context) : ICategoryRepository<EfCategory>
{
    public async Task<EfCategory?> GetCategoryAsync(int id)
    {
        var efCategory = await context
            .Categories
            .SingleOrDefaultAsync(x => x.Id == id);
        return efCategory;
    }

    public async Task<EfCategory?> GetCategoryAsync(string name)
    {
        var efCategory = await context
            .Categories
            .SingleOrDefaultAsync(x => x.Name == name);
        return efCategory;
    }

    public async Task<PagedResult<EfCategory>> GetCategoriesPagedAsync(int page, int pageSize)
    {
        var count = context.Categories.Count();
        var categories = await context
            .Categories
            .OrderBy(x => x.Id)
            .Skip(pageSize * page)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<EfCategory>(categories, count);
    }

    public async Task<EfCategory> CreateCategoryAsync(EfCategory category)
    {
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}