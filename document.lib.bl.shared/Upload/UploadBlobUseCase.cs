using document.lib.bl.contracts.Upload;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Upload;

public class UploadBlobUseCase(
    ILogger<UploadBlobUseCase> logger,
    DatabaseContext context,
    IUploadBlobCommand uploadBlobCommand,
    IAddToIndexCommand addToIndexCommand,
    IDeleteBlobCommand deleteBlobCommand): IUploadBlobUseCase
{
    const string NewDocumentsFolder = "unsorted";
    
    public async Task<Document?> ExecuteAsync(string filename, MemoryStream blob)
    {
        var blobName = Guid.NewGuid().ToString();
        var blobPath = $"{NewDocumentsFolder}/{blobName}";
        try
        {
            await context.Database.BeginTransactionAsync();
            await uploadBlobCommand.ExecuteAsync(blobPath, blob);
            var doc = await addToIndexCommand.ExecuteAsync(filename, blobName, blobPath);
            await context.SaveChangesAsync();
            await context.Database.CommitTransactionAsync();
            return doc;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            await context.Database.RollbackTransactionAsync();
            await deleteBlobCommand.ExecuteAsync(blobPath);
            return null;
        }
    }
}