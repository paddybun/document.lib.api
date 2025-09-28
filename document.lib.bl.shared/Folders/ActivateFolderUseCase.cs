using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.core.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class ActivateFolderUseCase(
    ILogger<ActivateFolderUseCase> logger,
    IFolderQuery<UnitOfWork> folderQuery) : IActivateFolderUseCase<UnitOfWork>
{
    public async Task<Result<bool>> ExecuteAsync(UnitOfWork uow, ActivateFolderUseCaseParameters parameters)
    {
        try
        {
            await uow.BeginTransactionAsync();
            logger.LogDebug("Executing activate folder use case with parameters {@Parameters}", parameters);
            logger.LogInformation("Executing activate folder use case for folder ID {FolderId}", parameters.FolderId);

            // Get all folders to update their active status
            string[] ignored = [SystemConstants.UnsortedFolderName, SystemConstants.DigitalFolderName];
            var allFolders = await uow.Connection.Folders
                .Where(x => !ignored.Contains(x.Name))
                .ToListAsync();

            // Set the target folder to active
            var folderToActivate = allFolders.FirstOrDefault(f => f.Id == parameters.FolderId);
            if (folderToActivate == null)
            {
                await uow.RollbackTransactionAsync();
                return Result<bool>.Warning("Folder not found");
            }

            folderToActivate.IsActive = true;

            // Set all eligible folders to inactive
            foreach (var folder in allFolders.Where(x => x.Id != parameters.FolderId))
            {
                folder.IsActive = false;
            }

            // Update all modified folders
            uow.Connection.UpdateRange(allFolders);
            await uow.CommitAsync();

            logger.LogInformation("Successfully activated folder with ID {FolderId} and deactivated all other eligible folders",
                parameters.FolderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogDebug("Error activating folder with parameters {@Parameters}", parameters);
            logger.LogError(ex, "Error activating folder with ID {FolderId}", parameters.FolderId);
            await uow.RollbackTransactionAsync();
            return Result<bool>.Failure($"An error occurred while activating the folder: {ex.Message}");
        }
    }
}
