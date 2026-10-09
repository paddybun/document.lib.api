using document.lib.bl.contracts.Tags.Queries;
using document.lib.bl.contracts.Tags.UseCases;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Tags.UseCases;

public class GetTagsUseCase(ILogger<GetTagsUseCase> logger, ITagsQuery<UnitOfWork> tagsQuery) : IGetTagsUseCase<UnitOfWork>
{
    public async Task<Result<IEnumerable<Tag>>> ExecuteAsync(UnitOfWork uow)
    {
        try
        {
            var result = await tagsQuery.ExecuteAsync(uow);
            return result.HasError ? Result<IEnumerable<Tag>>.Failure(result.Message) : result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting tags");
            return Result<IEnumerable<Tag>>.Failure(ex.Message, ex);
        }
    }
}
