using Azure.Storage.Blobs;
using document.lib.bl.contracts.Upload.Commands;

namespace document.lib.bl.shared.Upload.Commands;

public class CopyBlobCommand(BlobServiceClient blobServiceClient) : ICopyBlobCommand
{
    public async Task<bool> ExecuteAsync(string sourcePath, string destinationPath)
    {
        var container = blobServiceClient.GetBlobContainerClient("library-storage");
        var source = container.GetBlobClient(sourcePath);
        if (!await source.ExistsAsync()) return false;

        using var stream = new MemoryStream();
        await source.DownloadToAsync(stream);
        stream.Position = 0;

        await container.GetBlobClient(destinationPath).UploadAsync(stream, overwrite: true);
        return true;
    }
}
