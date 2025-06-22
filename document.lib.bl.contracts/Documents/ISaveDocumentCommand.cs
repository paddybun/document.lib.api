using document.lib.data.entities;

namespace document.lib.bl.contracts.Documents;

public interface ISaveDocumentCommand
{
    public Task<Document> ExecuteAsync(Document document);
}