using document.lib.bl.contracts.Documents.ViewModels;
using document.lib.bl.shared;
using document.lib.core.Models;
using document.lib.web.v2.Extensions;
using Radzen;
using Radzen.Blazor;

namespace document.lib.web.v2.Components.Pages.Documents;

public partial class DocumentOverview
{
    private IList<DocumentOverviewModel> _documents = null!;
    private int _count;
    private RadzenDataGrid<DocumentOverviewModel> _grid = null!;
    private string _lastFilter = string.Empty;

    private async Task LoadData(LoadDataArgs arg)
    {
        var uow = await UnitOfWork.CreateAsync(DbContextFactory);

        var filters = arg.Filters.Select(x => x.ToFilterString()).ToArray();
        
        var result = await DocumentListUseCase.ExecuteAsync(uow, new OverviewRequestParameters
        {
            Skip = arg.Skip ?? 0,
            Take = arg.Top ?? 0,
            Filter = filters
        });

        _count = result.Value!.Count;
        
        if (!result.HasData)
            return;
        
        _documents = result.Value!.FilteredList.ToList();
    }
    
    private void Callback(DataGridRowMouseEventArgs<DocumentOverviewModel> obj)
    {
        NavigationManager.NavigateTo($"{ManagedPages.Documents}/{obj.Data.Id}");
    }
}