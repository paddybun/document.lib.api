using Azure.Storage.Blobs;

namespace document.lib.shared.Services;

public class BlobClientHelper(BlobContainerClient blobContainerClient)
{
    public async Task UploadBlobAsync(string name, Stream buffer)
    {
        var blobClient = blobContainerClient.GetBlobClient(name);
        if (!await blobClient.ExistsAsync())
        {
            await blobClient.UploadAsync(buffer);
        }
    }

    public async Task<Stream> DownloadBlobAsync(string blob)
    {
        var blobClient = blobContainerClient.GetBlobClient(blob);
        var blobInfo = await blobClient.DownloadAsync();
        return blobInfo.Value.Content;
    }
}