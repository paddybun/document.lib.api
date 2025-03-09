using document.lib.ef.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Radzen.Blazor;

namespace document.lib.web.v2.Components.Pages;

public partial class DocumentOverview : ComponentBase
{
    private ICollection<EfDocument> _documents = null!;
    private int _count;
    private RadzenDataGrid<EfDocument> _grid;
    private string _lastFilter = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private async Task LoadData(LoadDataArgs arg)
    {
        var query = Context.Documents.AsQueryable().AsNoTracking();
        if (!string.IsNullOrEmpty(arg.Filter))
        {
            _lastFilter = arg.Filter;
            query = query.Where(_grid.ColumnsCollection);
            _count = await query.CountAsync();
        }
        else
        {
            _count = await Context.Documents.CountAsync();
        }
        
        query = query.OrderBy(x => x.Id);
        query = query.Skip(arg.Skip ?? 0).Take(arg.Top ?? 0);
        _documents = await query.ToListAsync();
    }
}