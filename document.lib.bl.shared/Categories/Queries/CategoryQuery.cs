using document.lib.bl.contracts.Categories.Queries;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories.Queries;

public class CategoryQuery(ILogger<CategoryQuery> logger): ICategoryQuery<UnitOfWork>
{
    public async Task<Result<Category>> ExecuteAsync(UnitOfWork uow, CategoryQueryParameters parameters)
    {
        logger.LogDebug("Retrieving category with name: {CategoryName}", parameters.CategoryName);

        var category = await uow.Connection.Categories
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == parameters.CategoryName);

        if (category == null)
        {
            logger.LogDebug("CategoryQuery no result for category name: {CategoryName}", parameters.CategoryName);
            return Result<Category>.Warning($"Category '{parameters.CategoryName}' not found.");
        }

        return Result<Category>.Success(category);
    }
}