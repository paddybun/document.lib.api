using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface IFolderRepository
{
    Task<DocLibFolder> GetFolderByNameAsync(string folderName);
    Task<DocLibFolder> GetFolderByIdAsync(string id);
    Task<DocLibFolder> GetActiveFolderAsync();
    Task<List<DocLibFolder>> GetAllFoldersAsync();
    Task<DocLibFolder> CreateFolderAsync(DocLibFolder folder);
    Task UpdateFolderAsync(DocLibFolder folder);
    Task AddDocumentToFolderAsync(DocLibFolder folder, DocLibDocument document);
    Task RemoveDocFromFolderAsync(DocLibFolder folder, DocLibDocument document);
}