using document.lib.core;
using document.lib.data.entities;
using document.lib.data.models.Folders;

namespace document.lib.bl.contracts.Folders;

public interface ISaveFolderUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<Folder>> ExecuteAsync(T unitOfWork, SaveFolderUseCaseParameters parameters);
}

public record SaveFolderUseCaseParameters(FolderSaveModel Folder);