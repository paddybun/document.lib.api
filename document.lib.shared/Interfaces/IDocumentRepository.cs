using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Interfaces;

public interface IDocumentRepository
{
    Task<DocumentModel> CreateDocumentAsync(DocumentModel document);
    Task<DocumentModel> GetDocumentAsync(DocumentQueryParameters queryParameters);
    Task<List<DocumentModel>> GetDocumentsAsync(int page, int count);
    Task<int> GetDocumentCountAsync();
    Task<List<DocumentModel>> GetDocumentsForFolderAsync(string folderName, int page, int count);
    Task<DocumentModel> UpdateDocumentAsync(DocumentModel document, CategoryModel category = null, FolderModel folder = null, TagModel[] tags = null);
    Task DeleteDocumentAsync(DocumentModel doc);
    Task DeleteDocumentAsync(string documentId);
}