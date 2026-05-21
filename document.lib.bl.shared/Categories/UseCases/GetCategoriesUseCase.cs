using document.lib.bl.contracts.Categories.Queries;
using document.lib.bl.contracts.Categories.UseCases;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories.UseCases;

public class GetCategoriesUseCase(
    ILogger<GetCategoriesUseCase> logger,
    ICategoriesQuery<UnitOfWork> categoriesQuery) : IGetCategoriesUseCase<UnitOfWork>
{
    public async Task<Result<List<Category>>> ExecuteAsync(UnitOfWork uow, GetCategoriesUseCaseParameters parameters)
    {
        try
        {
            logger.LogDebug("Getting categories with Skip: {Skip}, Take: {Take}", parameters.Skip, parameters.Take);

            var result = await categoriesQuery.ExecuteAsync(uow, new CategoriesQueryParameters(parameters.Skip, parameters.Take));

            if (result.HasError)
                return Result<List<Category>>.Failure(result.Message);

            logger.LogDebug("Successfully retrieved {Count} categories", result.Value?.Count ?? 0);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting categories");
            return Result<List<Category>>.Failure("An error occurred while retrieving categories", ex);
        }
    }
}
