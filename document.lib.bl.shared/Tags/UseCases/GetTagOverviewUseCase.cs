using document.lib.bl.contracts.Tags.Queries;
using document.lib.bl.contracts.Tags.UseCases;
using document.lib.bl.contracts.Tags.ViewModels;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Tags.UseCases;

public class GetTagOverviewUseCase(
    ILogger<GetTagOverviewUseCase> logger,
    ITagOverviewQuery<UnitOfWork> tagOverviewQuery) : IGetTagOverviewUseCase<UnitOfWork>
{
    public async Task<Result<List<TagOverviewModel>>> ExecuteAsync(UnitOfWork uow)
    {
        try
        {
            var result = await tagOverviewQuery.ExecuteAsync(uow);
            return result.HasError ? Result<List<TagOverviewModel>>.Failure(result.Message) : result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting the tag overview");
            return Result<List<TagOverviewModel>>.Failure("An error occurred while retrieving tags", ex);
        }
    }
}
