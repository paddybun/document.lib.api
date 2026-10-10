using document.lib.bl.contracts.Categories.UseCases;
using document.lib.bl.contracts.Categories.ViewModels;
using document.lib.bl.shared;
using Radzen;

namespace document.lib.web.v2.Components.Pages.Categories;

public partial class CategoryCreate
{
    private readonly CategoryOverviewModel _model = new();
    private bool _saving;

    private async Task Create()
    {
        if (_saving) return;

        _saving = true;
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await CreateCategoryUseCase.ExecuteAsync(uow,
            new CreateCategoryUseCaseParameters(_model.DisplayName ?? string.Empty));
        _saving = false;

        if (result.IsSuccess)
        {
            NotificationService.Notify(NotificationSeverity.Success, "Category created");
            NavigationManager.NavigateTo($"/{ManagedPages.Categories}/{result.Value}");
        }
        else
        {
            NotificationService.Notify(
                result.HasWarning ? NotificationSeverity.Warning : NotificationSeverity.Error,
                result.HasWarning ? result.Message : "Category could not be created");
        }
    }
}
