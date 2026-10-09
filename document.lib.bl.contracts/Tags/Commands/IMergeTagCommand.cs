using document.lib.core;

namespace document.lib.bl.contracts.Tags.Commands;

public interface IMergeTagCommand<in T>
    where T : IUnitOfWork
{
    /// <returns>The number of documents which got the destination tag through the merge.</returns>
    Task<Result<int>> ExecuteAsync(T uow, MergeTagCommandParameters parameters);
}

public record MergeTagCommandParameters(int SourceTagId, int DestinationTagId);
