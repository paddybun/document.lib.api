using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class SaveFolderUseCase (
    ILogger<SaveFolderUseCase> logger, 
    IFolderQuery<UnitOfWork> folderQuery): ISaveFolderUseCase<UnitOfWork>
{
    public async Task<Result<Folder>> ExecuteAsync(UnitOfWork unitOfWork, SaveFolderUseCaseParameters parameters)
    {
        try
        {
            logger.LogDebug("Executing folder query with parameters {@Parameters}", parameters);
            logger.LogInformation("Executing folder query");

            Folder folder;
            if (parameters.Folder.CreateNew)
            {
                folder = new Folder
                {
                    Name = Guid.NewGuid().ToString(),
                    MaxDocumentsRegister = parameters.Folder.MaxDocumentsRegister,
                    MaxDocumentsFolder = parameters.Folder.MaxDocumentsFolder,
                    DescriptionGroup = parameters.Folder.DescriptionGroup,
                    DisplayName = parameters.Folder.DisplayName,
                    IsActive = false
                };
                unitOfWork.Connection.Add(folder);
            }
            else
            {
                var folderQueryResult = await folderQuery.ExecuteAsync(unitOfWork, new FolderQueryParameters
                {
                    Id = parameters.Folder.Id
                });
                
                if (folderQueryResult is not { IsSuccess: true, Value: {} })
                    return Result<Folder>.Failure("Could not find folder to update");
                
                folder = parameters.Folder.ApplyToEntity(folderQueryResult.Value);
                unitOfWork.Connection.Update(folder);
            }

            await unitOfWork.CommitAsync();
            return Result<Folder>.Success(folder);
        }
        catch (Exception ex)
        {
            logger.LogDebug("Error saving folder with parameters {@Parameters}", parameters);
            logger.LogError(ex, "Error saving folder");
            return Result<Folder>.Failure(ex.Message);
        }
    }
}