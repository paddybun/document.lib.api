using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface IFolderService
{
    DocLibFolder GetActiveFolder();
    List<DocLibFolder> GetAll();
    Task UpdateFolderAsync(DocLibFolder folder);
}