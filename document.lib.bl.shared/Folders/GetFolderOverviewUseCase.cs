using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.models.Folders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class GetFolderOverviewUseCase(
    ILogger<GetFolderOverviewUseCase> logger) : IGetFolderOverviewUseCase<UnitOfWork>
{
    public async Task<Result<FolderView>> ExecuteAsync(UnitOfWork uow, GetFolderOverviewUseCaseParameters parameters)
    {
        try
        {
            logger.LogDebug("Getting folder overview for folder ID: {FolderId}", parameters.FolderId);

            var folder = await uow.Connection.Folders
                .Include(f => f.Registers)
                    .ThenInclude(r => r.Documents)
                .FirstOrDefaultAsync(f => f.Id == parameters.FolderId);

            if (folder == null)
            {
                logger.LogWarning("Folder with ID {FolderId} not found", parameters.FolderId);
                return Result<FolderView>.Warning("Folder not found");
            }

            var items = new List<FolderViewItem>();
            foreach (var folderRegister in folder.Registers)
            {
                var docs = folderRegister.Documents
                    .Select(x => new FolderViewItem
                    {
                        DocumentId = x.Id,
                        Document = x.DisplayName ?? string.Empty,
                        RegisterId = folderRegister.Id,
                        Register = folderRegister.DisplayName ?? string.Empty
                    });
                items.AddRange(docs);
            }
            
            var folderView = new FolderView
            {
                FolderId = folder.Id,
                Items = items.OrderBy(x => x.Register).ThenBy(x => x.Document).ToList()
            };

            logger.LogDebug("Successfully retrieved folder overview for folder ID: {FolderId} with {RegisterCount} registers", 
                parameters.FolderId, folderView.Items.Count);

            return Result<FolderView>.Success(folderView);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting folder overview for folder ID: {FolderId}", parameters.FolderId);
            return Result<FolderView>.Failure("An error occurred while retrieving the folder overview", ex);
        }
    }
}
