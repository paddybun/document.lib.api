using document.lib.bl.contracts.Folders.Queries;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Folders.Queries;

public class ActiveFolderQuery : IActiveFolderQuery<UnitOfWork>
{
    public async Task<Result<Folder>> ExecuteAsync(UnitOfWork uow)
    {
        var folder = await uow.Connection.Folders.AsNoTracking().FirstOrDefaultAsync(x => x.IsActive);
        return folder == null
            ? Result<Folder>.Warning("No active folder found")
            : Result<Folder>.Success(folder);
    }
}
