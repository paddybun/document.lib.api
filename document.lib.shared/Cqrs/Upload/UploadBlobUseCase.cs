using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Cqrs.Interfaces;
using document.lib.shared.Cqrs.Interfaces.Upload;

namespace document.lib.shared.Cqrs.Upload;

public class UploadBlobUseCase(
    DocumentLibContext context,
    IUploadBlobCommand uploadBlobCommand,
    IAddToIndexCommand addToIndexCommand,
    IDeleteBlobCommand deleteBlobCommand): IUploadBlobUseCase
{
    const string NewDocumentsFolder = "unsorted";
    
    public async Task<EfDocument?> ExecuteAsync(string filename, MemoryStream blob)
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