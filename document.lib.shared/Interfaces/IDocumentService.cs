using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface IDocumentService
{
    Task<DocumentModel?> GetDocumentByIdAsync(int id);
    Task<DocumentModel?> GetDocumentByNameAsync(string name);
    Task<(int, List<DocumentModel>)> GetDocumentsPagedAsync(int page, int pageSize);
    Task<(int, List<DocumentModel>)> GetUnsortedDocuments(int page, int pageSize);
    Task MoveDocumentAsync(int documentId, int folderFromId, int toFolderId);
    Task DeleteDocumentAsync(DocumentModel doc);
    Task<DocumentModel?> UpdateDocumentAsync(DocumentModel doc);
    Task<DocumentModel> CreateNewDocumentAsync(DocumentModel doc);
}