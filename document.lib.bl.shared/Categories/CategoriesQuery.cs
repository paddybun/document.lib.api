using document.lib.bl.contracts.Categories;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories;

public class CategoriesQuery(ILogger<CategoriesQuery> logger) : ICategoriesQuery<UnitOfWork>
{
    public async Task<Result<List<Category>>> ExecuteAsync(UnitOfWork uow, CategoriesQueryParameters parameters)
    {
        var query = uow.Connection.Categories.AsQueryable();

        if (parameters.Skip is {} skip)
        {
            query = query.Skip(skip);
        }

        if (parameters.Take is {} take)
        {
            query = query.Take(take);
        }

        var categories = await query.ToListAsync();
        return Result<List<Category>>.Success(categories);
    }
}