using document.lib.bl.contracts.Folders;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Folders;

public class FolderQuery(DatabaseContext context) :IFolderQuery
{
    public async Task<Folder?> ExecuteAsync(string folderName)
    {
        return await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleOrDefaultAsync(x => x.Name == folderName);
    }
}