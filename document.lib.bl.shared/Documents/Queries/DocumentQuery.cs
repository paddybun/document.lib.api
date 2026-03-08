using document.lib.bl.contracts.Documents.Queries;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Documents.Queries;

public class DocumentQuery: IDocumentQuery<UnitOfWork>
{
    public async Task<Result<Document>> ExecuteAsync(UnitOfWork uow, int id)
    {
        var entity = await uow.Connection.Documents
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Register)
            .ThenInclude(x => x.Description)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            return Result<Document>.Warning("Document not found");
        }
        
        return Result<Document>.Success(entity!);
    }
}