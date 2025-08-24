using document.lib.core;

namespace document.lib.bl.contracts.Folders;

public interface IDeleteFolderUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<bool>> ExecuteAsync(T unitOfWork, DeleteFolderUseCaseParameters parameters);
}

public record DeleteFolderUseCaseParameters
{
    public required int FolderId { get; init; }
}
