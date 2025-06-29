using document.lib.bl.contracts.Upload;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Upload;

public class AddToIndexCommand(DatabaseContext context): IAddToIndexCommand
{
    const string NewDocumentCategory = "uncategorized";
    const string NewFolderName = "unsorted";
    
    public async Task<Document> ExecuteAsync(string originalFilename, string blobName, string blobPath)
    {
        var name = Guid.NewGuid().ToString();
            
        var folder = await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleAsync(f => f.Name == NewFolderName);
        
        var category = await context.Categories.SingleAsync(f => f.Name == NewDocumentCategory);
        
        var doc = new Document
        {
            Name = name,
            PhysicalName = blobName,
            OriginalFileName = originalFilename,
            BlobLocation = blobPath,
            UploadDate = DateTimeOffset.Now,
            Register = folder.CurrentRegister!,
            Unsorted = true,
            Category = category
        };

        await context.Documents.AddAsync(doc);
        return doc;
    }
}