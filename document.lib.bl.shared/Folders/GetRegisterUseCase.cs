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
            await uow.BeginTransactionAsync();
            
            var folder = await uow.Connection.Folders.FindAsync(parameters.FolderId);
            if (folder == null) return Result<Register>.Warning("Folder not found");
            
            var registers = await uow.Connection.Registers
                .Include(x => x.Description)
                .Where(x => x.FolderId == parameters.FolderId)
                .OrderBy(x => x.Description.Order)
                .ToListAsync();

            var nextRegister = registers
                .FirstOrDefault(x => x.DocumentCount < folder.MaxDocumentsRegister);

            var shouldSave = nextRegister == null;
            if (registers.Count <= 0 && nextRegister == null)
            {
                var newFolderDescriptionResult = await nextDescription.ExecuteAsync(uow,
                    new() { Group = folder.DescriptionGroup, Id = default, IsNew = true });
                
                if (newFolderDescriptionResult is not { IsSuccess: true, Value: { } })
                    return Result<Register>.Warning("Could not get first description");
                
                nextRegister = new Register
                {
                    Name = newFolderDescriptionResult.Value.Name,
                    DisplayName = newFolderDescriptionResult.Value.DisplayName,
                    DescriptionId = newFolderDescriptionResult.Value!.Id,
                    FolderId = parameters.FolderId,
                    DocumentCount = 1
                };
            }
            else if (registers.Count > 0 && nextRegister == null)
            {
                var existingFolderDescriptionResult = await nextDescription.ExecuteAsync(uow, new()
                {
                    Id = registers.Last().Description.Id,
                    Group = folder.DescriptionGroup,
                    IsNew = false
                });
                
                if (existingFolderDescriptionResult is not { IsSuccess: true, Value: { } })
                    return Result<Register>.Warning("Could not get next description");
                
                var newRegister = new Register
                {
                    Name = existingFolderDescriptionResult.Value.Name,
                    DisplayName = existingFolderDescriptionResult.Value.DisplayName,
                    DescriptionId = existingFolderDescriptionResult.Value!.Id,
                    FolderId = parameters.FolderId,
                    DocumentCount = 1
                };
                nextRegister = newRegister;
            }
            else if (nextRegister == null)
            {
                return Result<Register>.Warning("No more definitions available for this folder");
            }

            if (shouldSave)
            {
                uow.Connection.Registers.Add(nextRegister);
                await uow.CommitAsync();
            }
            
            return Result<Register>.Success(nextRegister);
        }
        catch (Exception ex)
        {
            await uow.RollbackTransactionAsync();
            logger.LogError(ex, "Error getting next register for folder {FolderId}", parameters.FolderId);
            return Result<Register>.Failure();
        }
    }
}