using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Documents;

public interface ISaveDocumentUseCase
{
    public Task<Result<Document>> ExecuteAsync(Document document);
}