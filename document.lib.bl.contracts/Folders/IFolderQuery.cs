using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface IFolderQuery
{
    Task<Result<Folder>> ExecuteAsync(FolderQueryParameters parameters);
}

public class FolderQueryParameters
{
    public int? Id { get; set; }
    public string? FolderName { get; set; }
}