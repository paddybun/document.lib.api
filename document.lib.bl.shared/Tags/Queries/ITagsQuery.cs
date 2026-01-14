using document.lib.bl.contracts.Tags.Queries;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Tags.Queries;

public class TagsQuery(ILogger<TagsQuery> logger): ITagsQuery<UnitOfWork>
{
    public async Task<Result<IEnumerable<Tag>>> ExecuteAsync(UnitOfWork uow)
    {
        try
        {
            logger.LogInformation("Executing Tags Query");

            var tags = await uow.Connection.Tags
                .AsNoTracking()
                .OrderBy(x => x.DisplayName)
                .ToListAsync();
            
            return Result<IEnumerable<Tag>>.Success(tags);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occured while fetching Tags");
            return Result<IEnumerable<Tag>>.Failure(ex.Message);
        }
    }
}