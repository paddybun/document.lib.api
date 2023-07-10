using document.lib.ef.Entities;
using document.lib.shared.TableEntities;
using DocLibDocument = document.lib.shared.TableEntities.DocLibDocument;

namespace document.lib.shared.Interfaces;

public interface IDocumentRepository
{
    Task<DocLibDocument> CreateDocumentAsync(DocLibDocument document);
    Task<DocLibDocument> GetDocumentById(string id);
    Task<DocLibDocument> GetDocumentByName(string name);
    Task<List<DocLibDocument>> GetDocuments(int page, int count);
    Task<int> GetDocumentCount();
    Task<List<DocLibDocument>> GetDocumentsForFolder(string folderName, int page, int count);
    Task<DocLibDocument> UpdateDocumentAsync(DocLibDocument document, DocLibCategory category = null, DocLibFolder folder = null, DocLibTag[] tags = null);
    Task DeleteDocumentAsync(DocLibDocument doc);
    Task DeleteDocumentAsync(string documentId);
}