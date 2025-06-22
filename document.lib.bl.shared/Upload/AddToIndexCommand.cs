using document.lib.bl.contracts.Upload;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Upload;

public class AddToIndexCommand(DatabaseContext context): IAddToIndexCommand
{
    const string NewDocumentCategory = "uncategorized";
    const string NewDocumentRegister = "unsorted";
    
    public async Task<Document> ExecuteAsync(string fileName, string blobPath)
    {
        var name = Guid.NewGuid().ToString();
            
        var folder = await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleAsync(f => f.Name == NewDocumentRegister);
        var category = await context.Categories.SingleAsync(f => f.Name == NewDocumentCategory);
        
        var doc = new Document
        {
            Name = name,
            PhysicalName = fileName,
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