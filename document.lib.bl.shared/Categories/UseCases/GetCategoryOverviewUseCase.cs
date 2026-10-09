using document.lib.bl.contracts.Categories.Queries;
using document.lib.bl.contracts.Categories.UseCases;
using document.lib.bl.contracts.Categories.ViewModels;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories.UseCases;

public class GetCategoryOverviewUseCase(
    ILogger<GetCategoryOverviewUseCase> logger,
    ICategoryOverviewQuery<UnitOfWork> categoryOverviewQuery) : IGetCategoryOverviewUseCase<UnitOfWork>
{
    public async Task<Result<List<CategoryOverviewModel>>> ExecuteAsync(UnitOfWork uow, GetCategoryOverviewUseCaseParameters parameters)
    {
        try
        {
            var result = await categoryOverviewQuery.ExecuteAsync(uow, new CategoryOverviewQueryParameters(parameters.CategoryId));
            return result.HasError ? Result<List<CategoryOverviewModel>>.Failure(result.Message) : result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting category overview");
            return Result<List<CategoryOverviewModel>>.Failure("An error occurred while retrieving categories", ex);
        }
    }
}
