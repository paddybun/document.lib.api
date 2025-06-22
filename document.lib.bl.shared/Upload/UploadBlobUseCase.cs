using document.lib.bl.contracts.Upload;
using document.lib.data.context;
using document.lib.data.entities;

namespace document.lib.bl.shared.Upload;

public class UploadBlobUseCase(
    DatabaseContext context,
    IUploadBlobCommand uploadBlobCommand,
    IAddToIndexCommand addToIndexCommand,
    IDeleteBlobCommand deleteBlobCommand): IUploadBlobUseCase
{
    const string NewDocumentsFolder = "unsorted";
    
    public async Task<Document?> ExecuteAsync(string filename, MemoryStream blob)
    {
        var blobPath = $"{NewDocumentsFolder}/{filename}";
        try
        {
            await context.Database.BeginTransactionAsync();
            await uploadBlobCommand.ExecuteAsync(blobPath, blob);
            var doc = await addToIndexCommand.ExecuteAsync(filename, blobPath);
            await context.SaveChangesAsync();
            await context.Database.CommitTransactionAsync();
            return doc;
        }
        catch (Exception ex)
        {
            // TODO: Use MS Logger
            Console.WriteLine(ex.Message);
            await context.Database.RollbackTransactionAsync();
            await deleteBlobCommand.ExecuteAsync(blobPath);
            return null;
        }
    }
}