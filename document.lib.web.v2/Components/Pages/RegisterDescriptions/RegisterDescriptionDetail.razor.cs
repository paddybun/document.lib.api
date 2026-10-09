using document.lib.bl.contracts.RegisterDescriptions.UseCases;
using document.lib.bl.shared;
using document.lib.data.models.RegisterDescriptions;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace document.lib.web.v2.Components.Pages.RegisterDescriptions;

public partial class RegisterDescriptionDetail
{
    [Parameter] public string? Group { get; set; }
    [SupplyParameterFromQuery] public string? CopyFrom { get; set; }

    private bool _isNew;
    private bool _loaded;
    private bool _saving;
    private bool _inUse;
    private string _originalGroup = string.Empty;
    private string _groupName = string.Empty;
    private List<RegisterDescriptionEntryModel> _entries = [];
    private string _customText = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        _isNew = string.IsNullOrEmpty(Group);
        if (_isNew)
        {
            if (!string.IsNullOrWhiteSpace(CopyFrom))
                await LoadCopyAsync(CopyFrom);

            _loaded = true;
            StateHasChanged();
            return;
        }

        await LoadAsync(Group!);
    }

    private async Task LoadAsync(string group)
    {
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await RegisterDescriptionQuery.ExecuteAsync(uow, new() { GroupName = group });
        if (!result.IsSuccess || result.Value == null)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Register set could not be loaded");
            Back();
            return;
        }

        Apply(result.Value);
        _loaded = true;
        StateHasChanged();
    }

    private async Task LoadCopyAsync(string source)
    {
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await RegisterDescriptionQuery.ExecuteAsync(uow, new() { GroupName = source });
        if (!result.IsSuccess || result.Value == null)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Register set to copy could not be loaded");
            return;
        }

        _groupName = $"{source} (copy)";
        _entries = result.Value.Entries.OrderBy(x => x.Order)
            .Select(x => new RegisterDescriptionEntryModel { DisplayName = x.DisplayName })
            .ToList();
    }

    private void Copy() => NavigationManager.NavigateTo($"/RegisterSets/Create?CopyFrom={Uri.EscapeDataString(_originalGroup)}", forceLoad: true);

    private void Apply(RegisterDescriptionDetailModel model)
    {
        _originalGroup = model.Group;
        _inUse = model.InUse;
        _groupName = model.Group;
        _entries = model.Entries.OrderBy(x => x.Order).ToList();
    }

    private void Generate()
    {
        var names = RegisterSetTemplates.FromText(_customText);

        if (names.Count == 0)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Please enter at least one entry");
            return;
        }

        _entries = names.Select((x, i) => new RegisterDescriptionEntryModel { DisplayName = x, Order = i }).ToList();
    }

    private void AddEntry() => _entries.Add(new RegisterDescriptionEntryModel { DisplayName = string.Empty });

    private void Remove(int index) => _entries.RemoveAt(index);

    private void Move(int index, int offset)
    {
        var target = index + offset;
        if (target < 0 || target >= _entries.Count) return;
        (_entries[index], _entries[target]) = (_entries[target], _entries[index]);
    }

    private async Task Save()
    {
        var name = _groupName.Trim();
        var model = new RegisterDescriptionSaveModel
        {
            GroupName = _isNew ? name : _originalGroup,
            NewGroupName = _isNew ? null : name,
            CreateNew = _isNew,
            Entries = _entries
                .Select((x, i) => new RegisterDescriptionEntryModel { Id = x.Id, DisplayName = x.DisplayName, Order = i })
                .ToList()
        };

        _saving = true;
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await RegisterDescriptionSaveUseCase.ExecuteAsync(uow, new() { SaveModel = model });
        _saving = false;

        if (!result.IsSuccess || result.Value == null)
        {
            NotificationService.Notify(NotificationSeverity.Error,
                result.HasWarning ? result.Message : "Register set could not be saved");
            return;
        }

        NotificationService.Notify(NotificationSeverity.Success, _isNew ? "Register set created" : "Register set saved");

        if (_isNew || result.Value.Group != _originalGroup)
        {
            NavigationManager.NavigateTo($"/{ManagedPages.Description}/{Uri.EscapeDataString(result.Value.Group)}", forceLoad: true);
            return;
        }

        Apply(result.Value);
    }

    private void Back() => NavigationManager.NavigateTo($"/{ManagedPages.Description}");
}
