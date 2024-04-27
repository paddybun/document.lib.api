using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface IDocumentService
{
    Task<DocumentModel?> GetDocumentByIdAsync(int id);
    Task<DocumentModel?> GetDocumentAsync(string? id = null, string? name = null);
    Task<(int, List<DocumentModel>)> GetDocumentsPagedAsync(int page, int pageSize);
    Task<List<DocumentModel>> GetUnsortedDocuments();
    Task DeleteDocumentAsync(DocumentModel doc);
    Task<DocumentModel> UpdateDocumentAsync(DocumentModel doc);
    Task<DocumentModel> CreateNewDocumentAsync(DocumentModel doc);
    Task<bool> MoveDocumentAsync(DocumentModel doc);
}