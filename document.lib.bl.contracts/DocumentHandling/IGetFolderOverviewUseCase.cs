using document.lib.core;
using document.lib.data.models.Folders;

namespace document.lib.bl.contracts.DocumentHandling;

public interface IGetFolderOverviewUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<FolderView>> ExecuteAsync(T uow, GetFolderOverviewUseCaseParameters folderId);
}

public record GetFolderOverviewUseCaseParameters(int FolderId);
