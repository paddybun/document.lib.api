using document.lib.bl.contracts.Categories.Queries;
using document.lib.bl.contracts.Categories.UseCases;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories.UseCases;

public class GetCategoryUseCase(
    ILogger<GetCategoryUseCase> logger,
    ICategoryQuery<UnitOfWork> categoryQuery) : IGetCategoryUseCase<UnitOfWork>
{
    public async Task<Result<Category>> ExecuteAsync(UnitOfWork uow, GetCategoryUseCaseParameters parameters)
    {
        try
        {
            logger.LogDebug("Getting category with name: {CategoryName}", parameters.CategoryName);

            var result = await categoryQuery.ExecuteAsync(uow, new CategoryQueryParameters(parameters.CategoryName));

            if (result.HasError)
                return Result<Category>.Failure(result.Message);

            logger.LogDebug("Successfully retrieved category with name: {CategoryName}", parameters.CategoryName);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting category with name: {CategoryName}", parameters.CategoryName);
            return Result<Category>.Failure("An error occurred while retrieving the category", ex);
        }
    }
}
