using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface IFoldersQuery<in T>
    where T : IUnitOfWork
{
    Task<Result<List<Folder>>> ExecuteAsync(T uow);
}