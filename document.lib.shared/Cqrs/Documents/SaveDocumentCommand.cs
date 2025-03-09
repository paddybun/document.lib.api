using document.lib.ef.Entities;
using document.lib.shared.Cqrs.Interfaces;

namespace document.lib.shared.Cqrs.Documents;

public class SaveDocumentCommand: ISaveDocumentCommand
{
    public Task<EfDocument> ExecuteAsync(EfDocument document)
    {
        throw new NotImplementedException();
    }
}