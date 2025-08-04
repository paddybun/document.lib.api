using document.lib.bl.contracts.Documents;
using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Documents;

public class SaveDocumentUseCase(
    ILogger<SaveDocumentUseCase> logger,
    DatabaseContext context,
    IGetRegisterUseCase getRegisterUseCase): ISaveDocumentUseCase
{
    public async Task<Result<Document>> ExecuteAsync(Document document, int folderId)
    {
        try
        {
            logger.LogInformation("Saving document {id}", document.Id);
            
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
                serverDoc.Register.DocumentCount--;
                var register = await getRegisterUseCase.ExecuteAsync(folder.Id);
                if (register is not { IsSuccess: true, Value: not null }) return Result<Document>.Failure();
                
                register.Value.DocumentCount++;
                serverDoc.Register = register.Value;
            }
            
            return Result<Document>.Failure();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving document {id}", document.Id);
            return Result<Document>.Failure();
        }
    }
}