using document.lib.core;

namespace document.lib.bl.contracts.Folders;

public interface IActivateFolderUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<bool>> ExecuteAsync(T uow, ActivateFolderUseCaseParameters parameters);
}

public record ActivateFolderUseCaseParameters
{
    public required int FolderId { get; init; }
}
