using document.lib.ef.Entities;

namespace document.lib.shared.Cqrs.Interfaces;

public interface IAddToIndexCommand
{
    Task<EfDocument> ExecuteAsync(string fileName, string blobPath);
}