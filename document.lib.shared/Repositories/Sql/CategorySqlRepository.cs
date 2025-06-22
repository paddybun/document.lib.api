using document.lib.data.entities;
using document.lib.ef;
using document.lib.shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class CategorySqlRepository(DocumentLibContext context) : ICategoryRepository<Category>
{
    public async Task<Category?> GetCategoryAsync(int id)
    {
        var efCategory = await context
            .Categories
            .SingleOrDefaultAsync(x => x.Id == id);
        return efCategory;
    }

    public async Task<Category?> GetCategoryAsync(string name)
    {
        var efCategory = await context
            .Categories
            .SingleOrDefaultAsync(x => x.Name == name);
        return efCategory;
    }

    public async Task<PagedResult<Category>> GetCategoriesPagedAsync(int page, int pageSize)
    {
        var count = context.Categories.Count();
        var categories = await context
            .Categories
            .OrderBy(x => x.Id)
            .Skip(pageSize * page)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Category>(categories, count);
    }

    public async Task<Category> CreateCategoryAsync(Category category)
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