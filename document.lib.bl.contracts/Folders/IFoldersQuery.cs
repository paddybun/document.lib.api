using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface IFoldersQuery
{
    Task<Result<List<Folder>>> ExecuteAsync();
}