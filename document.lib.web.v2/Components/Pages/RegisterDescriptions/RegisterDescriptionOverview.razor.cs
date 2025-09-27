using document.lib.bl.shared;
using document.lib.data.entities;

namespace document.lib.web.v2.Components.Pages.RegisterDescriptions;

public partial class RegisterDescriptionOverview
{
    private List<RegisterDescription> _registerDescriptions = [];
    private Dictionary<string, int> _registerDescriptionsGroups = [];
    
    private string _selectedGroup = string.Empty;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory); 
        if (firstRender)
        {
            var descriptionsResult = await RegisterDescriptionsQuery.ExecuteAsync(uow, new() { HideSystemDescriptions = false });
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
        NavigationManager.NavigateTo($"{ManagedPages.Description}/{_selectedGroup}");
    }
}