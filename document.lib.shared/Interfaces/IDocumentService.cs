using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Interfaces;

public interface IDocumentService
{
    Task DeleteDocumentAsync(DocumentModel doc);
    Task<DocumentModel> UpdateDocumentAsync(DocumentModel doc);
    Task<DocumentModel> CreateNewDocumentAsync(DocumentModel doc);
    Task<bool> MoveDocumentAsync(DocumentModel doc);
}