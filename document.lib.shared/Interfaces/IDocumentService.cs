using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface IDocumentService
{
    Task DeleteDocumentAsync(DocLibDocument doc);
    Task<DocLibDocument> UpdateDocumentAsync(DocLibDocument doc);
    Task<DocLibDocument> CreateDocumentAsync(DocLibDocument doc);
    Task<bool> MoveDocumentAsync(DocLibDocument doc);
}