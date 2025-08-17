using document.lib.bl.contracts.Folders;
using document.lib.bl.shared;
using document.lib.data.entities;
using document.lib.data.models.Folders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace document.lib.web.v2.Components.Pages.Folders;

public partial class FolderDetail : ComponentBase
{
    [Parameter] public int Id { get; set; }

    private Folder _folder = null!;
    private FolderView? _folderView = null;
    private IEnumerable<FolderViewItem> _items = null!;
    private RadzenDataGrid<FolderViewItem> grid;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
            var folderQuery = await FolderQuery.ExecuteAsync(uow, new FolderQueryParameters { Id = Id });
            if (folderQuery is not { IsSuccess: true, Value: not null })
            {
                NotificationService.Notify(new ()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = "Could not load folder information"
                });
            }
            _folder = folderQuery.Value!;
            StateHasChanged();
        }
    }

    private async Task LoadFolderOverview(MouseEventArgs arg)
    {
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var folderOverviewResult = await GetFolderOverviewUseCase.ExecuteAsync(uow, new GetFolderOverviewUseCaseParameters(Id));
        
        if (folderOverviewResult is { IsSuccess: true, Value: not null })
        {
            _folderView = folderOverviewResult.Value;
            _items = _folderView.Items;
            StateHasChanged();
        }
        else
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Error NOTT",
                Detail = "Could not load folder overview NOTT"
            });
        }
    }

    private void OnRender(DataGridRenderEventArgs<FolderViewItem> args)
    {
        if(args.FirstRender)
        {
            args.Grid.Groups.Add(new GroupDescriptor { Property = nameof(FolderViewItem.Register), SortOrder = SortOrder.Ascending });
            StateHasChanged();
        }
    }
}