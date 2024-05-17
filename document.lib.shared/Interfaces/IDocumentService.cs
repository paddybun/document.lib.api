using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;

namespace document.lib.shared.Interfaces;

public interface IDocumentService
{
    Task<ITypedServiceResult<DocumentModel>> GetDocumentAsync(int id);
    Task<ITypedServiceResult<DocumentModel>> GetDocumentAsync(string name);
    Task<ITypedServiceResult<PagedResult<DocumentModel>>> GetDocumentsPagedAsync(int page, int pageSize);
    Task<(int, List<DocumentModel>)> GetUnsortedDocuments(int page, int pageSize);
    Task MoveDocumentAsync(int documentId, int folderFromId, int toFolderId);
    Task DeleteDocumentAsync(DocumentModel doc);
    Task<DocumentModel?> ModifyTagsAsync(int id, string[] toAdd, string[] toRemove);
    Task<DocumentModel?> UpdateDocumentAsync(DocumentModel doc);
    Task<DocumentModel> AddDocumentToIndexAsync(string blobPath);
    Task<ITypedServiceResult<DocumentModel>> CreateDocumentAsync(DocumentModel doc);
}

public record PagedResult<T>(IReadOnlyList<T> Results, int Total);