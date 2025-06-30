using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class FoldersQuery(
    ILogger<FoldersQuery> logger,
    DatabaseContext context): IFoldersQuery
{
    public async Task<Result<List<Folder>>> ExecuteAsync()
    {
        try
        {
            logger.LogDebug("Retrieving folders");
            logger.LogInformation("FoldersQuery running...");

            var folders = await context.Folders
                .AsNoTracking()
                .ToListAsync();

            return Result<List<Folder>>.Success(folders);
        } 
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            logger.LogDebug("FoldersQuery failed");
            return Result<List<Folder>>.Failure();
        }
    }
}