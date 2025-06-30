using document.lib.bl.contracts.Documents;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Documents;

public class SaveDocumentUseCase(
    ILogger<SaveDocumentUseCase> logger,
    DatabaseContext context): ISaveDocumentUseCase
{
    public async Task<Result<Document>> ExecuteAsync(Document document, int folderId)
    {
        try
        {
            var serverDoc = await context.Documents.AsNoTracking()
                .Include(x => x.Register)
                .ThenInclude(x => x.Folder)
                .SingleAsync(x => x.Id == document.Id);
            
            
            var folder = await context.Folders
                .AsNoTracking()
                .Include(x => x.Registers)
                .SingleAsync(x => x.Id == folderId);
            
            var moveToNewFolder = folder.Registers.All(x => x.Id != serverDoc.Register.Id);

            if (moveToNewFolder)
            {
                // Transfer document to new folder
            }
            
            
            
            return Result<Document>.Failure();
        }
        catch (Exception ex)
        {
            return Result<Document>.Failure();
        }
    }
}