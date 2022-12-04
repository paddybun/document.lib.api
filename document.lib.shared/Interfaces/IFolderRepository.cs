using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface IFolderRepository
{
    DocLibFolder GetFolderByName(string folderName);
    DocLibFolder GetFolderById(string id);
    DocLibFolder GetCurrentlyActiveFolder();
    List<DocLibFolder> GetAllFolders();
    Task UpdateNameAsync(DocLibFolder folder);
}