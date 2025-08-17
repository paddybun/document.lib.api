using document.lib.bl.shared;
using document.lib.data.entities;

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
}