using document.lib.bl.contracts.Categories;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories;

public class CategoryQuery(ILogger<CategoriesQuery> logger, DatabaseContext context): ICategoryQuery
{
    public async Task<Result<Category>> ExecuteAsync(string categoryName)
    {
        try
        {
            logger.LogDebug("Retrieving category with name: {CategoryName}", categoryName);
            logger.LogInformation("CategoryQuery running...");
            
            var category = await context.Categories
                .SingleOrDefaultAsync(x => x.Name == categoryName);

            if (category == null)
            {
                logger.LogWarning("CategoryQuery no result");
                logger.LogDebug("CategoryQuery no result for category name: {CategoryName}", categoryName);
                return Result<Category>.Warning($"Category '{categoryName}' not found.");
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug("CategoryQuery failed for '{CategoryName}'", categoryName);
            logger.LogError(ex, "CategoryQuery failed for category name: {CategoryName}", categoryName);
            return Result<Category>.Failure();
        }
    }
}