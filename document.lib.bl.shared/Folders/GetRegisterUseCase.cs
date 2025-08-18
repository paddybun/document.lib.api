using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class GetRegisterUseCase(
    ILogger<GetRegisterUseCase> logger, 
    INextDescriptionQuery<UnitOfWork> nextDescription): IGetRegisterUseCase<UnitOfWork>
{
    public async Task<Result<Register>> ExecuteAsync(UnitOfWork uow, GetRegisterUseCaseParameters parameters)
    {
        try
        {
            var folder = await uow.Connection.Folders.FindAsync(parameters.FolderId);
            if (folder == null) return Result<Register>.Warning("Folder not found");
            
            var registers = await uow.Connection.Registers
                .Include(x => x.Description)
                .Where(x => x.FolderId == parameters.FolderId)
                .OrderBy(x => x.Description.Order)
                .ToListAsync();

            var nextRegister = registers
                .FirstOrDefault(x => x.DocumentCount < folder.MaxDocumentsRegister);

            if (registers.Count == 0 && nextRegister == null)
            {
                var d = await nextDescription.ExecuteAsync(uow, new() { Group = "default", Id = default, IsNew = true});
                if (!d.IsSuccess) return Result<Register>.Warning("Could not get first description");
                nextRegister = new Register
                {
                    DescriptionId = d.Value!.Id,
                    FolderId = parameters.FolderId,
                    DocumentCount = 1
                };
            }
            else if (registers.Count > 0 && nextRegister == null)
            {
                var nextDescriptionResult = await nextDescription.ExecuteAsync(uow, new()
                {
                    Id = registers.Last().Description.Id,
                    Group = registers.Last().Description.Group,
                    IsNew = false
                });
                if (!nextDescriptionResult.IsSuccess) return Result<Register>.Warning("Could not get next description");
                var newRegister = new Register
                {
                    DescriptionId = nextDescriptionResult.Value!.Id,
                    FolderId = parameters.FolderId,
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
            logger.LogError(ex, "Error getting next register for folder {FolderId}", parameters.FolderId);
            return Result<Register>.Failure();
        }
    }
}