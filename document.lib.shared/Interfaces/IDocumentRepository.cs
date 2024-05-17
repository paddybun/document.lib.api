using document.lib.shared.Models.Data;

namespace document.lib.shared.Interfaces;

public interface IDocumentRepository<T>: IRepository
{
    Task<T?> GetDocumentAsync(int id);
    Task<T?> GetDocumentAsync(string name);
    Task<T> CreateDocumentAsync(T document);
    Task<PagedResult<T>> GetDocumentsPagedAsync(int page, int pageSize);
    Task<PagedResult<T>> GetUnsortedDocumentsAsync(int page, int pageSize);
    Task<PagedResult<T>> GetDocumentsForFolderAsync(string folderName, int page, int pageSize);
    Task<T> UpdateDocumentAsync(DocumentModel document, int? category = null, FolderModel? folder = null, TagModel[]? tags = null);
    Task DeleteDocumentAsync(T doc);
}