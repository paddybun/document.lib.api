using document.lib.bl.contracts.Categories.UseCases;
using document.lib.bl.contracts.Categories.ViewModels;
using document.lib.bl.shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace document.lib.web.v2.Components.Pages.Categories;

public partial class CategoryEdit : ComponentBase
{
    [Parameter] public int Id { get; set; }

    private CategoryOverviewModel? _category;
    private bool _loaded;
    private bool _saving;
    private bool _merging;
    private bool _deleting;
    private int? _destinationId;
    private List<CategoryOverviewModel> _otherCategories = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await GetCategoryOverviewUseCase.ExecuteAsync(uow, new GetCategoryOverviewUseCaseParameters(Id));

        _category = result.IsSuccess ? result.Value?.FirstOrDefault() : null;

        var all = await GetCategoryOverviewUseCase.ExecuteAsync(uow, new GetCategoryOverviewUseCaseParameters());
        _otherCategories = (all.Value ?? []).Where(x => x.Id != Id).ToList();
        foreach (var other in _otherCategories) other.DisplayName ??= other.Name;
        _destinationId = null;
        if (!result.IsSuccess)
            NotificationService.Notify(NotificationSeverity.Error, "Category could not be loaded");

        _loaded = true;
        StateHasChanged();
    }

    private async Task Save()
    {
        if (_category == null) return;

        _saving = true;
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await UpdateCategoryUseCase.ExecuteAsync(uow,
            new UpdateCategoryUseCaseParameters(_category.Id, _category.DisplayName ?? string.Empty));
        _saving = false;

        if (result.IsSuccess)
        {
            NotificationService.Notify(NotificationSeverity.Success, "Category saved");
            await ReloadAsync();
        }
        else
            NotificationService.Notify(NotificationSeverity.Error, "Category could not be saved");
    }

    private async Task Merge()
    {
        var destination = _otherCategories.FirstOrDefault(x => x.Id == _destinationId);
        if (_category == null || destination == null) return;

        var confirmed = await DialogService.Confirm(
            $"Move all documents of '{_category.DisplayName ?? _category.Name}' to '{destination.DisplayName}'?",
            "Merge category",
            new ConfirmOptions { OkButtonText = "Merge", CancelButtonText = "Cancel" });
        if (confirmed != true) return;

        _merging = true;
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await MergeCategoryUseCase.ExecuteAsync(uow, new MergeCategoryUseCaseParameters(_category.Id, destination.Id));
        _merging = false;

        if (result.IsSuccess)
        {
            NotificationService.Notify(NotificationSeverity.Success, $"{result.Value} document(s) moved to '{destination.DisplayName}'");
            await ReloadAsync();
        }
        else
            NotificationService.Notify(NotificationSeverity.Error, "Categories could not be merged");
    }

    private async Task Delete()
    {
        if (_category is not { DocumentCount: 0 }) return;

        var confirmed = await DialogService.Confirm(
            $"Do you really want to delete the category '{_category.DisplayName ?? _category.Name}'?",
            "Delete category",
            new ConfirmOptions { OkButtonText = "Delete", CancelButtonText = "Cancel" });
        if (confirmed != true) return;

        _deleting = true;
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await DeleteCategoryUseCase.ExecuteAsync(uow, new DeleteCategoryUseCaseParameters(_category.Id));
        _deleting = false;

        if (result.IsSuccess)
        {
            NotificationService.Notify(NotificationSeverity.Success, "Category deleted");
            Back();
        }
        else
            NotificationService.Notify(NotificationSeverity.Error, "Category could not be deleted");
    }

    private void Back() => NavigationManager.NavigateTo($"/{ManagedPages.Categories}");
}
