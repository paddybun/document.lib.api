using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Cqrs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Cqrs.Categories;

public class CategoryQuery(DocumentLibContext context): ICategoryQuery
{
    public async Task<EfCategory?> ExecuteAsync(string categoryName)
    {
        return await context.Categories
            .SingleOrDefaultAsync(x => x.Name == categoryName);
    }
}