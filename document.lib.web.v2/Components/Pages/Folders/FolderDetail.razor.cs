using document.lib.bl.contracts.Folders;
using document.lib.bl.shared;
using document.lib.data.entities;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace document.lib.web.v2.Components.Pages.Folders;

public partial class FolderDetail : ComponentBase
{
    [Parameter] public int Id { get; set; }

    private Folder _folder = null!;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var uow = await UnitOfWork.CreateAsync(DbContextFactory);
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
}