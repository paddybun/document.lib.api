using document.lib.bl.contracts.Categories;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories;

public class CategoryQuery(ILogger<CategoriesQuery> logger): ICategoryQuery<UnitOfWork>
{
    public async Task<Result<Category>> ExecuteAsync(UnitOfWork uow, CategoryQueryParameters parameters)
    {
        try
        {
            logger.LogDebug("Retrieving category with name: {CategoryName}", parameters.CategoryName);
            logger.LogInformation("CategoryQuery running...");
            
            var category = await uow.Connection.Categories
                .SingleOrDefaultAsync(x => x.Name == parameters.CategoryName);

            if (category == null)
            {
                logger.LogWarning("CategoryQuery no result");
                logger.LogDebug("CategoryQuery no result for category name: {CategoryName}", parameters.CategoryName);
                return Result<Category>.Warning($"Category '{parameters.CategoryName}' not found.");
            }
            
            return Result<Category>.Success(category);
        }
        catch (Exception ex)
        {
            logger.LogDebug("CategoryQuery failed for '{CategoryName}'", parameters.CategoryName);
            logger.LogError(ex, "CategoryQuery failed for category name: {CategoryName}", parameters.CategoryName);
            return Result<Category>.Failure();
        }
    }
}