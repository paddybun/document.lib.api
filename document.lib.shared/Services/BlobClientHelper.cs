using Azure.Identity;
using Azure.Storage.Blobs;

namespace document.lib.shared.Services;

public class BlobClientHelper
{
    private readonly BlobContainerClient _blobContainerClient;

    public BlobClientHelper(string blobContainerConnectionString, string container)
    {
        _blobContainerClient = new BlobContainerClient(blobContainerConnectionString, container);
    }

    public async Task UploadBlobAsync(string name, Stream buffer)
    {
        var blobClient = _blobContainerClient.GetBlobClient(name);
        if (!await blobClient.ExistsAsync())
        {
            await blobClient.UploadAsync(buffer);
        }
    }

    public async Task<Stream> DownloadBlobAsync(string blob)
    {
        var blobClient = _blobContainerClient.GetBlobClient(blob);
        var blobInfo = await blobClient.DownloadAsync();
        return blobInfo.Value.Content;
    }
}