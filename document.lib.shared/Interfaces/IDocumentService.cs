using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface IDocumentService
{
    Task<DocumentModel?> GetDocumentAsync(string? id = null, string? name = null);
    Task<List<DocumentModel>> GetAllDocumentsAsync();
    Task<List<DocumentModel>> GetUnsortedDocuments();
    Task DeleteDocumentAsync(DocumentModel doc);
    Task<DocumentModel> UpdateDocumentAsync(DocumentModel doc);
    Task<DocumentModel> CreateNewDocumentAsync(DocumentModel doc);
    Task<bool> MoveDocumentAsync(DocumentModel doc);
}