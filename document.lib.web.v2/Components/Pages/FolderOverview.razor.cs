using document.lib.data.entities;
using Microsoft.AspNetCore.Components;

namespace document.lib.web.v2.Components.Pages;

public partial class FolderOverview : ComponentBase
{
    private List<Folder> _folders = null!;
    private IList<Folder> _selectedFolders = new List<Folder>();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var foldersResult = await FoldersQuery.ExecuteAsync();
            _folders = foldersResult.IsSuccess ? foldersResult.Value! : [];
            StateHasChanged();
        }
    }
}