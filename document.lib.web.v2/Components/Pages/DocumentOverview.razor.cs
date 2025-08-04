using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Radzen.Blazor;

namespace document.lib.web.v2.Components.Pages;

public partial class DocumentOverview
{
    private ICollection<Document> _documents = null!;
    private int _count;
    private RadzenDataGrid<Document> _grid = null!;
    private string _lastFilter = string.Empty;

    private async Task LoadData(LoadDataArgs arg)
    {
        await using var context = await DbContextFactory.CreateDbContextAsync();
        var query = context.Documents.AsQueryable().AsNoTracking();
        if (!string.IsNullOrEmpty(arg.Filter))
        {
            _lastFilter = arg.Filter;
            query = query.Where(_grid.ColumnsCollection);
            _count = await query.CountAsync();
        }
        else
        {
            _count = await context.Documents.CountAsync();
        }
        
        query = query.OrderBy(x => x.Id);
        query = query.Skip(arg.Skip ?? 0).Take(arg.Top ?? 0);
        _documents = await query.ToListAsync();
    }
}