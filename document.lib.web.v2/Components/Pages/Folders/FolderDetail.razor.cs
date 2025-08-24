using document.lib.bl.contracts.DocumentHandling;
using document.lib.bl.contracts.Folders;
using document.lib.bl.shared;
using document.lib.data.entities;
using document.lib.data.models.Folders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.WebUtilities;
using Radzen;
using Radzen.Blazor;

namespace document.lib.web.v2.Components.Pages.Folders;

public partial class FolderDetail : ComponentBase
{
    [Parameter] public int Id { get; set; }
    
    private bool _createMode = false;
    private List<string> _registerDescriptions = null;
    private string _selectedDescription = null;
    
    
    private Folder _editModeModel = null!;
    private bool _isActive = false;
    private FolderSaveModel _createModeModel = null!;
    private FolderView? _folderView = null;
    private IEnumerable<FolderViewItem> _items = null!;
    private RadzenDataGrid<FolderViewItem> _grid;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);
            _createMode = query.TryGetValue("mode", out var mode) && mode == "create";

            if (_createMode)
            {
                var registersResult = await RegisterDescriptionsQuery.ExecuteAsync(uow, new());
                if (registersResult is not { IsSuccess: true, Value: { }})
                {
                    // TODO: Notify user about error
                    return;
                }
                
                _registerDescriptions = registersResult.Value
                    .GroupBy(x => x.Group)
                    .Select(x => x.Key)
                    .ToList();
                
                _createModeModel = new FolderSaveModel(true)
                {
                    DescriptionGroup = string.Empty
                };
            }
            else
            {
                
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
                _editModeModel = folderQuery.Value!;
                _isActive = _editModeModel.IsActive;
            }
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

    private async Task CreateNewFolder()
    {
        var uow = await UnitOfWork.CreateAsync(DbContextFactory);

        if (string.IsNullOrWhiteSpace(_selectedDescription))
        {
            // TODO: Replace with Radzen RequiredValidator
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = "Warning",
                Detail = "Please select a description group for the folder."
            });
            return;
        }
        _createModeModel.DescriptionGroup = _selectedDescription;
        
        var createResult = await SaveFolderUseCase.ExecuteAsync(uow, new() { Folder = _createModeModel});
        
        if (createResult is { IsSuccess: true, Value: not null })
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Folder created",
                Detail = "The folder has been successfully created."
            });
            NavigationManager.NavigateTo($"{ManagedPages.Folder}/{createResult.Value.Id}");
        }
        else
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Error",
                Detail = "Could not create folder"
            });
        }
    }

    private async Task DeleteFolder(int id)
    {
        var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        await DeleteFolderUseCase.ExecuteAsync(uow, new (){  FolderId = id});
    }

    private async Task MakeFolderActive(bool arg)
    {
        var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var activateResult = await ActivateFolderUseCase.ExecuteAsync(uow, new() { FolderId = Id });
        
        if (activateResult is { IsSuccess: true, Value: true })
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Folder activated",
                Detail = "The folder has been successfully activated."
            });
            _isActive = true;
            StateHasChanged();
        }
        else
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Error",
                Detail = "Could not activate folder"
            });
            _isActive = false;
            StateHasChanged();
        }
    }
}