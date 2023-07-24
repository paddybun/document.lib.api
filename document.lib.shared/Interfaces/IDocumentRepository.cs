using document.lib.shared.Models.QueryDtos;
using document.lib.shared.TableEntities;
using DocLibDocument = document.lib.shared.TableEntities.DocLibDocument;

namespace document.lib.shared.Interfaces;

public interface IDocumentRepository
{
    Task<DocLibDocument> CreateDocumentAsync(DocLibDocument document);
    Task<DocLibDocument> GetDocumentAsync(DocumentQueryParameters queryParameters);
    Task<List<DocLibDocument>> GetDocumentsAsync(int page, int count);
    Task<int> GetDocumentCountAsync();
    Task<List<DocLibDocument>> GetDocumentsForFolderAsync(string folderName, int page, int count);
    Task<DocLibDocument> UpdateDocumentAsync(DocLibDocument document, DocLibCategory category = null, DocLibFolder folder = null, DocLibTag[] tags = null);
    Task DeleteDocumentAsync(DocLibDocument doc);
    Task DeleteDocumentAsync(string documentId);
}