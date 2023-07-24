using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface IFolderService
{
    Task<DocLibFolder> GetFolderByNameAsync(string name);
    Task<DocLibFolder> GetOrCreateActiveFolderAsync();
    Task<DocLibFolder> CreateNewFolderAsync(int maxFolderSize = 200, int maxRegisterSize = 10);
    Task<DocLibFolder> GetOrCreateFolderByIdAsync(string name, int maxFolderSize = 200, int maxRegisterSize = 10);
    Task <List<DocLibFolder>> GetAllAsync();
    Task UpdateFolderAsync(DocLibFolder folder);
    Task AddDocumentToFolderAsync(DocLibFolder folder, DocLibDocument doc);
    Task RemoveDocumentFromFolder(DocLibFolder folder, DocLibDocument doc);
}