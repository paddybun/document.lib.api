using document.lib.bl.contracts.Documents;
using document.lib.data.entities;

namespace document.lib.bl.shared.Documents;

public class SaveDocumentCommand: ISaveDocumentCommand
{
    public Task<Document> ExecuteAsync(Document document)
    {
        throw new NotImplementedException();
    }
}