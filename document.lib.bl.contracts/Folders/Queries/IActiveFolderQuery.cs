using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders.Queries;

public interface IActiveFolderQuery<in T> where T : IUnitOfWork
{
    Task<Result<Folder>> ExecuteAsync(T uow);
}
