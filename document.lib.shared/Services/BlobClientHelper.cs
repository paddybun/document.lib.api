using Azure.Storage.Blobs;
using document.lib.shared.Models;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Services;

public class BlobClientHelper(IOptions<AppConfiguration> config)
{
    private readonly BlobContainerClient _blobContainerClient = new(config.Value.BlobServiceConnectionString, config.Value.BlobContainer);

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