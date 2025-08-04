using document.lib.core;
using document.lib.data.entities;
using document.lib.data.models.Documents;

namespace document.lib.bl.contracts.Documents;

public interface ISaveDocumentUseCase
{
    public Task<Result<Document>> ExecuteAsync(SaveDocumentUseCaseParameters parameters);
}

public class SaveDocumentUseCaseParameters
{
    public required int DocumentId { get; set; }
    public required int FolderId { get; set; }
    public required DocumentSaveModel Document { get; set; }
}