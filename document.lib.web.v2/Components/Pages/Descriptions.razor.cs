using document.lib.data.entities;
using Microsoft.AspNetCore.Components;

namespace document.lib.web.v2.Components.Pages;

public partial class Descriptions : ComponentBase
{
    private List<RegisterDescription> _registerDescriptions = [];
    private Dictionary<string, int> _registerDescriptionsGroups = [];
    
    private string _selectedGroup = string.Empty;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var descriptionsResult = await RegisterDescriptionsQuery.ExecuteAsync();
            _registerDescriptions = descriptionsResult.Value ?? [];
            
            _registerDescriptionsGroups = _registerDescriptions
                .GroupBy(x => x.Group)
                .ToDictionary(g => g.Key, g => g.Count());
            
            StateHasChanged();
        }
    }

    private void Edit(string context)
    {
        _selectedGroup = context;
    }
}