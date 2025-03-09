using Azure.Storage.Blobs;
using document.lib.shared.Cqrs.Interfaces.Upload;

namespace document.lib.shared.Cqrs.Upload;

public class DeleteBlobCommand(BlobServiceClient blobServiceClient): IDeleteBlobCommand
{
    public async Task<bool> ExecuteAsync(string blobPath)
    {
        var bcc = blobServiceClient.GetBlobContainerClient("library-storage");
        var bc = bcc.GetBlobClient(blobPath);
        
        if (!await bc.ExistsAsync()) return false;
        await bc.DeleteAsync();
        
        return true;
    }
}