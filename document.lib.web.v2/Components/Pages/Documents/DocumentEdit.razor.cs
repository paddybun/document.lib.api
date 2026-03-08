using Microsoft.AspNetCore.Components;

namespace document.lib.web.v2.Components.Pages.Documents;

public partial class DocumentEdit : ComponentBase
{
    [Parameter] public int Id { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Load document
            StateHasChanged();
        }
    }
}