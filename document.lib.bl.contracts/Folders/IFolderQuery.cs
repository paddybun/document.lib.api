using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface IFolderQuery<in T>
    where T : IUnitOfWork
{
    Task<Result<Folder>> ExecuteAsync(T uow, FolderQueryParameters parameters);
}

public class FolderQueryParameters
{
    public int? Id { get; set; }
    public string? FolderName { get; set; }
}