using document.lib.bl.contracts.Folders;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class DeleteFolderUseCase(
    ILogger<DeleteFolderUseCase> logger,
    IFolderQuery<UnitOfWork> folderQuery) : IDeleteFolderUseCase<UnitOfWork>
{
    public async Task<Result<bool>> ExecuteAsync(UnitOfWork unitOfWork, DeleteFolderUseCaseParameters parameters)
    {
        try
        {
            logger.LogDebug("Executing delete folder use case with parameters {@Parameters}", parameters);
            logger.LogInformation("Executing delete folder use case for folder ID {FolderId}", parameters.FolderId);

            var folderQueryResult = await folderQuery.ExecuteAsync(unitOfWork, new FolderQueryParameters
            {
                Id = parameters.FolderId
            });

            if (folderQueryResult is not { IsSuccess: true, Value: { } folder })
            {
                logger.LogWarning("Folder with ID {FolderId} not found", parameters.FolderId);
                return Result<bool>.Warning("Folder not found");
            }

            var hasRegisters = folder.Registers.Any();
            var hasDocuments = folder.Registers.SelectMany(r => r.Documents).Any();

            if (hasRegisters || hasDocuments)
            {
                logger.LogWarning("Cannot delete folder {FolderId} - folder is not empty. Has {RegisterCount} registers and {DocumentCount} documents", 
                    parameters.FolderId, folder.Registers.Count, folder.Registers.SelectMany(r => r.Documents).Count());
                return Result<bool>.Warning("Cannot delete folder - folder is not empty. Remove all registers and documents first.");
            }

            // Folder is empty, proceed with deletion
            unitOfWork.Connection.Remove(folder);
            await unitOfWork.CommitAsync();

            logger.LogInformation("Successfully deleted empty folder with ID {FolderId}", parameters.FolderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogDebug("Error deleting folder with parameters {@Parameters}", parameters);
            logger.LogError(ex, "Error deleting folder with ID {FolderId}", parameters.FolderId);
            return Result<bool>.Failure($"An error occurred while deleting the folder: {ex.Message}");
        }
    }
}
