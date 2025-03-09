using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Cqrs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Cqrs.Upload;

public class AddToIndexCommand(DocumentLibContext context): IAddToIndexCommand
{
    const string NewDocumentCategory = "uncategorized";
    const string NewDocumentRegister = "unsorted";
    
    public async Task<EfDocument> ExecuteAsync(string fileName, string blobPath)
    {
        var name = Guid.NewGuid().ToString();
            
        var folder = await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleAsync(f => f.Name == NewDocumentRegister);
        var category = await context.Categories.SingleAsync(f => f.Name == NewDocumentCategory);
        
        var doc = new EfDocument
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