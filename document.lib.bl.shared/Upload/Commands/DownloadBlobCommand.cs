using Azure.Storage.Blobs;
using document.lib.bl.contracts.Upload.Commands;

namespace document.lib.bl.shared.Upload.Commands;

public class DownloadBlobCommand(BlobServiceClient blobServiceClient) : IDownloadBlobCommand
{
    public async Task<Stream?> ExecuteAsync(string blobPath)
    {
        var container = blobServiceClient.GetBlobContainerClient("library-storage");
        var client = container.GetBlobClient(blobPath);
        if (!await client.ExistsAsync()) return null;

        return await client.OpenReadAsync();
    }
}
