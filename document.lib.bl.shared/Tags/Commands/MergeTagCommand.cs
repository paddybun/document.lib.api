using document.lib.bl.contracts.Tags.Commands;
using document.lib.core;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Tags.Commands;

public class MergeTagCommand : IMergeTagCommand<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, MergeTagCommandParameters parameters)
    {
        if (parameters.SourceTagId == parameters.DestinationTagId)
            return Result<int>.Warning("Source and destination tag must differ.");

        var ids = new[] { parameters.SourceTagId, parameters.DestinationTagId };
        var tags = await uow.Connection.Tags.Where(x => ids.Contains(x.Id)).ToListAsync();
        var source = tags.SingleOrDefault(x => x.Id == parameters.SourceTagId);
        var destination = tags.SingleOrDefault(x => x.Id == parameters.DestinationTagId);
        if (source == null || destination == null)
            return Result<int>.Warning("Source or destination tag not found.");

        var destinationDocumentIds = await uow.Connection.TagAssignments
            .Where(x => x.Tag.Id == destination.Id)
            .Select(x => x.Document.Id)
            .ToListAsync();
        var alreadyTagged = destinationDocumentIds.ToHashSet();

        var sourceAssignments = await uow.Connection.TagAssignments
            .Include(x => x.Document)
            .Where(x => x.Tag.Id == source.Id)
            .ToListAsync();

        var moved = 0;
        foreach (var assignment in sourceAssignments)
        {
            // documents which already carry the destination tag only lose the source tag
            if (alreadyTagged.Add(assignment.Document.Id))
            {
                assignment.Tag = destination;
                moved++;
            }
            else
            {
                uow.Connection.TagAssignments.Remove(assignment);
            }
        }

        uow.Connection.Tags.Remove(source);
        return Result<int>.Success(moved);
    }
}
