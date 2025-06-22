using Azure.Storage.Blobs;
using document.lib.bl.contracts.Upload;

namespace document.lib.bl.shared.Upload;

public class UploadBlobCommand(BlobServiceClient blobServiceClient): IUploadBlobCommand
{
    public async Task<bool> ExecuteAsync(string blobPath, MemoryStream blob)
    {
        var bc = blobServiceClient.GetBlobContainerClient("library-storage");
        var client = bc.GetBlobClient(blobPath);
        var exists = await client.ExistsAsync(); 
        if (!exists)
        {
            await client.UploadAsync(blob);
        }
        return true;
    }
}