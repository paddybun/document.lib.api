using document.lib.core;

namespace document.lib.bl.contracts.Tags.UseCases;

public interface IMergeTagUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, MergeTagUseCaseParameters parameters);
}

public record MergeTagUseCaseParameters(int SourceTagId, int DestinationTagId);
