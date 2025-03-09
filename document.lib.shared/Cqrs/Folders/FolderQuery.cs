using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Cqrs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Cqrs.Folders;

public class FolderQuery(DocumentLibContext context) :IFolderQuery
{
    public async Task<EfFolder?> ExecuteAsync(string folderName)
    {
        return await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleOrDefaultAsync(x => x.Name == folderName);
    }
}