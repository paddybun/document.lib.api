using document.lib.bl.contracts.Categories.UseCases;
using document.lib.bl.contracts.Categories.ViewModels;
using document.lib.bl.shared;
using Radzen;

namespace document.lib.web.v2.Components.Pages.Categories;

public partial class CategoryOverview
{
    private List<CategoryOverviewModel>? _categories;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await GetCategoryOverviewUseCase.ExecuteAsync(uow, new GetCategoryOverviewUseCaseParameters());
        if (result.IsSuccess)
            _categories = result.Value ?? [];
        else
        {
            _categories = [];
            NotificationService.Notify(NotificationSeverity.Error, "Categories could not be loaded");
        }

        StateHasChanged();
    }

    private void Edit(int id) => NavigationManager.NavigateTo($"/{ManagedPages.Categories}/{id}");
}
