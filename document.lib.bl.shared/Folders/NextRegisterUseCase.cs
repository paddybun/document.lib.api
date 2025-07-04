using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class NextRegisterUseCase(
    ILogger<NextRegisterUseCase> logger, 
    DatabaseContext context,
    INextDescriptionQuery nextDescription): INextRegisterUseCase
{
    public async Task<Result<Register>> ExecuteAsync(int folderId)
    {
        try
        {
            var folder = await context.Folders.FindAsync(folderId);
            if (folder == null) return Result<Register>.Warning("Folder not found");
            
            var registers = await context.Registers
                .Include(x => x.Description)
                .Where(x => x.FolderId == folderId)
                .OrderBy(x => x.Description.Order)
                .ToListAsync();

            var toReturn = registers
                .FirstOrDefault(x => x.DocumentCount < folder.MaxDocumentsRegister);

            // first document in folder
            if (registers.Count == 0 && toReturn == null)
            {
                var d = await nextDescription.ExecuteAsync(new() { Group = "default", Id = -1 });
                if (!d.IsSuccess) return Result<Register>.Warning("Could not get first description");
                toReturn = new Register
                {
                    DescriptionId = d.Value!.Id,
                    FolderId = folderId,
                    DocumentCount = 1
                };
            }
            else if (toReturn == null)
            {
                var nextDescriptionResult = await nextDescription.ExecuteAsync(new()
                {
                    Id = registers.Last().Description.Id,
                    Group = registers.Last().Description.Group
                });
                if (!nextDescriptionResult.IsSuccess) return Result<Register>.Warning("Could not get next description");
                var newRegister = new Register
                {
                    DescriptionId = nextDescriptionResult.Value!.Id,
                    FolderId = folderId,
                    DocumentCount = 1
                };
                toReturn = newRegister;
            }
            
            return Result<Register>.Success(toReturn);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting next register for folder {FolderId}", folderId);
            return Result<Register>.Failure();
        }
    }
}