using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface IDocumentRepository
{
    Task DeleteDocumentAsync(DocLibDocument doc);
    Task DeleteDocumentAsync(string documentId);
    Task UpdateFolderReferenceAsync(string folderId, string folderDisplayName);
}