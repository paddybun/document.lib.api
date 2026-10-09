using document.lib.bl.contracts.Tags.UseCases;
using document.lib.bl.contracts.Tags.ViewModels;
using document.lib.bl.shared;
using Radzen;

namespace document.lib.web.v2.Components.Pages.Tags;

public partial class TagOverview
{
    private List<TagOverviewModel>? _tags;
    private int? _sourceId;
    private int? _destinationId;
    private bool _merging;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await GetTagOverviewUseCase.ExecuteAsync(uow);

        if (!result.IsSuccess)
            NotificationService.Notify(NotificationSeverity.Error, "Tags could not be loaded");

        _tags = result.Value ?? [];
        _sourceId = null;
        _destinationId = null;
        StateHasChanged();
    }

    private void SelectSource(int id)
    {
        _sourceId = id;
        if (_destinationId == id) _destinationId = null;
    }

    private async Task Merge()
    {
        var source = _tags?.FirstOrDefault(x => x.Id == _sourceId);
        var destination = _tags?.FirstOrDefault(x => x.Id == _destinationId);
        if (source == null || destination == null) return;

        var confirmed = await DialogService.Confirm(
            $"Merge '{source.DisplayName}' into '{destination.DisplayName}'? All documents get the destination tag and '{source.DisplayName}' is deleted.",
            "Merge tags",
            new ConfirmOptions { OkButtonText = "Merge", CancelButtonText = "Cancel" });
        if (confirmed != true) return;

        _merging = true;
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await MergeTagUseCase.ExecuteAsync(uow, new MergeTagUseCaseParameters(source.Id, destination.Id));
        _merging = false;

        if (result.IsSuccess)
        {
            NotificationService.Notify(NotificationSeverity.Success, $"'{source.DisplayName}' merged into '{destination.DisplayName}'");
            await ReloadAsync();
        }
        else
            NotificationService.Notify(NotificationSeverity.Error,
                result.HasWarning ? result.Message : "Tags could not be merged");
    }
}
