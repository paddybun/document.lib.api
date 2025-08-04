using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class GetRegisterUseCase(
    ILogger<GetRegisterUseCase> logger, 
    DatabaseContext context,
    INextDescriptionQuery nextDescription): IGetRegisterUseCase
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

            var nextRegister = registers
                .FirstOrDefault(x => x.DocumentCount < folder.MaxDocumentsRegister);

            if (registers.Count == 0 && nextRegister == null)
            {
                var d = await nextDescription.ExecuteAsync(new() { Group = "default", Id = default, IsNew = true});
                if (!d.IsSuccess) return Result<Register>.Warning("Could not get first description");
                nextRegister = new Register
                {
                    DescriptionId = d.Value!.Id,
                    FolderId = folderId,
                    DocumentCount = 1
                };
            }
            else if (registers.Count > 0 && nextRegister == null)
            {
                var nextDescriptionResult = await nextDescription.ExecuteAsync(new()
                {
                    Id = registers.Last().Description.Id,
                    Group = registers.Last().Description.Group,
                    IsNew = false
                });
                if (!nextDescriptionResult.IsSuccess) return Result<Register>.Warning("Could not get next description");
                var newRegister = new Register
                {
                    DescriptionId = nextDescriptionResult.Value!.Id,
                    FolderId = folderId,
                    DocumentCount = 1
                };
                nextRegister = newRegister;
            }
            else
            {
                throw new InvalidOperationException("Could not get register");
            }

            return Result<Register>.Success(nextRegister);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting next register for folder {FolderId}", folderId);
            return Result<Register>.Failure();
        }
    }
}