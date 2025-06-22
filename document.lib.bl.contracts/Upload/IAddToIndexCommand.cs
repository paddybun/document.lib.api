using document.lib.data.entities;

namespace document.lib.bl.contracts.Upload;

public interface IAddToIndexCommand
{
    Task<Document> ExecuteAsync(string fileName, string blobPath);
}