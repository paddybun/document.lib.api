using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface IDocumentRepository
{
    Task<DocumentModel> CreateDocumentAsync(DocumentModel document);
    Task<DocumentModel?> GetDocumentAsync(DocumentModel model);
    Task<List<DocumentModel>> GetAllDocumentsAsync();
    Task<List<DocumentModel>> GetDocumentsPagedAsync(int lastId, int count);
    Task<List<DocumentModel>> GetUnsortedDocumentsAsync();
    Task<int> GetDocumentCountAsync();
    Task<List<DocumentModel>> GetDocumentsForFolderAsync(string folderName, int page, int count);
    Task<DocumentModel> UpdateDocumentAsync(DocumentModel document, CategoryModel? category = null, FolderModel? folder = null, TagModel[]? tags = null);
    Task DeleteDocumentAsync(DocumentModel doc);
    Task DeleteDocumentAsync(string documentId);
}