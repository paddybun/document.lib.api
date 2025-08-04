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
    public async Task<Result<Document>> ExecuteAsync(SaveDocumentUseCaseParameters parameters)
    {
        try
        {
            logger.LogInformation("Saving document {id}", parameters.DocumentId);
            
            var serverDoc = await context.Documents
                .Include(x => x.Register)
                .ThenInclude(x => x.Folder)
                .Include(x => x.Tags)
                .ThenInclude(x => x.Tag)
                .SingleAsync(x => x.Id == parameters.DocumentId);
            
            var folder = await context.Folders
                .AsNoTracking()
                .Include(x => x.Registers)
                .SingleAsync(x => x.Id == parameters.FolderId);
            
            var moveToNewFolder = folder.Registers.All(x => x.Id != serverDoc.Register.Id);
            if (moveToNewFolder)
            {
                serverDoc.Register.DocumentCount--;
                var register = await getRegisterUseCase.ExecuteAsync(folder.Id);
                if (register is not { IsSuccess: true, Value: not null }) return Result<Document>.Failure();
                
                register.Value.DocumentCount++;
                serverDoc.Register = register.Value;
            }
            
            serverDoc = parameters.Document.ToEntity(serverDoc);
            
            // TODO: compare tags and update accordingly
            
            await context.SaveChangesAsync();
            
            return Result<Document>.Success(serverDoc);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving document {id}", parameters.DocumentId);
            return Result<Document>.Failure();
        }
    }
}