using document.lib.bl.shared;
using document.lib.data.entities;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace document.lib.web.v2.Components.Pages.Folders;

public partial class FolderOverview
{
    private List<Folder> _folders = null!;
    private IList<Folder> _selectedFolders = new List<Folder>();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var uow = await UnitOfWork.CreateAsync(DbContextFactory);
            var foldersResult = await FoldersQuery.ExecuteAsync(uow);
            _folders = foldersResult.IsSuccess ? foldersResult.Value! : [];
            StateHasChanged();
        }
    }

    private static void OnRowRender(RowRenderEventArgs<Folder> args)
    {
        if (!args.Data.IsActive) return;

        args.Attributes["style"] = "background-color: var(--rz-success-lighter); font-weight: bold; border-left: 6px solid var(--rz-success);";
    }

    private void NavigateTo(Folder folder)
    {
        NavigationManager.NavigateTo($"{ManagedPages.Folder}/{folder.Id}");
    }

    private void CreateNewFolder()
    {
        NavigationManager.NavigateTo($"{ManagedPages.Folder}/0?mode=create");
    }
}