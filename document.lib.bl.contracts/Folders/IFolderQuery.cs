using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface IFolderQuery
{
    Task<Folder?> ExecuteAsync(string folderName);
}