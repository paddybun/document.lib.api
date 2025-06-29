using document.lib.data.entities;

namespace document.lib.bl.contracts.Upload;

public interface IAddToIndexCommand
{
    Task<Document> ExecuteAsync(string originalFilename, string blobName, string blobPath);
}