using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Categories;

public class CategoriesQuery(DatabaseContext context) : ICategoriesQuery
{
    public async Task<List<Category>> ExecuteAsync(int? skip = null, int? take = null)
    {
        var query = context.Categories.AsQueryable();

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync();
    }
}