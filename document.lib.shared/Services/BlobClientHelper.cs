using Azure.Storage.Blobs;
using document.lib.shared.Models;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Services;

public class BlobClientHelper
{
    private readonly BlobContainerClient _blobContainerClient;

    public BlobClientHelper(IOptions<AppConfiguration> config)
    {
        _blobContainerClient = new BlobContainerClient(config.Value.BlobContainerConnectionString, config.Value.BlobContainer);
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