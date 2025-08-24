using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class FolderQuery(ILogger<FolderQuery> logger): IFolderQuery<UnitOfWork>
{
    public async Task<Result<Folder>> ExecuteAsync(UnitOfWork uow, FolderQueryParameters parameters)
    {
        try
        {
            logger.LogInformation("Executing folder query with parameters {@Parameters}", parameters);
            if (parameters is { FolderName: null, Id: null }) return Result<Folder>.Failure("Please supply a folder name or id");

            Folder folder;
            var getByName = !string.IsNullOrWhiteSpace(parameters.FolderName) && parameters.Id is null or <= 0;
            
            if (getByName)
            {
                var f = await uow.Connection.Folders
                    .AsNoTracking()
                    .Include(x => x.Registers)
                    .ThenInclude(x => x.Documents)
                    .SingleOrDefaultAsync(x => x.Name == parameters.FolderName);
                if (f == null) return Result<Folder>.Failure("Folder not found");
                folder = f;
            }
            else
            {
                folder = await uow.Connection.Folders
                    .Include(x => x.Registers)
                    .ThenInclude(x => x.Documents)
                    .SingleAsync(x => x.Id == parameters.Id);
            }
            
            return Result<Folder>.Success(folder);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing folder query with parameters {@Parameters}", parameters);
            return Result<Folder>.Failure("An error occurred while executing the folder query");
        }
    }
}