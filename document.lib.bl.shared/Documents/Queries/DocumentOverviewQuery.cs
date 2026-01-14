using document.lib.bl.contracts.Documents.Queries;
using document.lib.core;
using document.lib.core.Extensions;
using document.lib.core.Models;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Documents.Queries;

public class DocumentOverviewQuery : IDocumentOverviewQuery<UnitOfWork>
{
    public async Task<Result<FilteredResult<Document>>> ExecuteAsync(UnitOfWork uow, OverviewRequestParameters parameters)
    {
        var pk = uow.Connection.Model.FindEntityType(typeof(Document))!.FindPrimaryKey()!;

        var entities = await uow.Connection.Documents
            .AsNoTracking()
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .AddQueryParameters(parameters, pk)
            .ToListAsync();

        var count = await uow.Connection.Documents
            .AddQueryParameters(parameters, pk, true)
            .CountAsync();
        
        return Result<FilteredResult<Document>>.Success(new()
        {
            Count = count,
            FilteredList = entities
        });
    }
}