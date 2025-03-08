using document.lib.ef.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace document.lib.web.v2.Components.Pages;

public partial class DocumentOverview : ComponentBase
{
    private ICollection<EfDocument> _documents = null!;
    private int _count;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private async Task LoadData(LoadDataArgs arg)
    {
        _count = await Context.Documents.Where(x => !x.Unsorted).CountAsync();
        var query = Context.Documents.AsQueryable().AsNoTracking();
        query = query.OrderBy(x => x.Id);
        query = query.Skip(arg.Skip ?? 0).Take(arg.Top ?? 0);
        _documents = await query.ToListAsync();
    }
}