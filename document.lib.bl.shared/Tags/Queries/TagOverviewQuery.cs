using document.lib.bl.contracts.Tags.Queries;
using document.lib.bl.contracts.Tags.ViewModels;
using document.lib.core;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Tags.Queries;

public class TagOverviewQuery : ITagOverviewQuery<UnitOfWork>
{
    public async Task<Result<List<TagOverviewModel>>> ExecuteAsync(UnitOfWork uow)
    {
        var tags = await uow.Connection.TagAssignments
            .AsNoTracking()
            .GroupBy(x => x.Tag.Id)
            .Select(g => new { TagId = g.Key, Count = g.Count() })
            .Join(uow.Connection.Tags.AsNoTracking(), g => g.TagId, t => t.Id,
                (g, t) => new TagOverviewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    DisplayName = t.DisplayName ?? t.Name,
                    DocumentCount = g.Count
                })
            .OrderBy(x => x.DisplayName)
            .ToListAsync();

        return Result<List<TagOverviewModel>>.Success(tags);
    }
}
