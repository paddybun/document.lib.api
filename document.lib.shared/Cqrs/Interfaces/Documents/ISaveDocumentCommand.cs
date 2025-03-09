using document.lib.ef.Entities;

namespace document.lib.shared.Cqrs.Interfaces;

public interface ISaveDocumentCommand
{
    public Task<EfDocument> ExecuteAsync(EfDocument document);
}