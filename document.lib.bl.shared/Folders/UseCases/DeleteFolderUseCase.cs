using document.lib.bl.contracts.Folders.Queries;
using document.lib.bl.contracts.Folders.UseCases;
using document.lib.core;
using document.lib.core.System;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders.UseCases;

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

            if (folder.Name is SystemConstants.UnsortedFolderName or SystemConstants.DigitalFolderName)
                return Result<bool>.Warning("System folders cannot be deleted.");

            var documentCount = folder.Registers.Sum(r => r.Documents.Count);
            if (documentCount > 0)
            {
                logger.LogWarning("Cannot delete folder {FolderId} - it contains {DocumentCount} documents in {RegisterCount} registers",
                    parameters.FolderId, documentCount, folder.Registers.Count);
                return Result<bool>.Warning($"The folder cannot be deleted because it still contains {documentCount} document(s). Move or delete them first.");
            }

            // Registers without documents are removed together with the folder
            unitOfWork.Connection.RemoveRange(folder.Registers);
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
