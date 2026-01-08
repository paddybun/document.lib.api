using System.Collections.ObjectModel;
using document.lib.bl.shared;
using document.lib.data.models.RegisterDescriptions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace document.lib.web.v2.Components.Pages.RegisterDescriptions;

public partial class RegisterDescriptionDetail
{
    [Parameter] public string Group { get; set; } = null!;

    private DataGridEditMode _editMode = DataGridEditMode.Single;
    private RadzenDataGrid<RegisterDescriptionEntryModel> _grid = null!;
    private RegisterDescriptionDetailModel? _model;
    private ObservableCollection<RegisterDescriptionEntryModel> _gridModel = null!;
    private IList<RegisterDescriptionEntryModel>? _selectedDescriptions;
    private RegisterDescriptionEntryModel _draggedItem = null!;
    private List<RegisterDescriptionEntryModel> _descriptionToUpdate;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
            var descriptionsResult = await RegisterDescriptionQuery.ExecuteAsync(uow, new() { GroupName = Group });
            if (descriptionsResult.HasWarning || !descriptionsResult.IsSuccess)
            {
                NotificationService.Notify(new()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = L["Messages.LoadError"],
                    Detail = L["Descriptions.NotFoundMessage"],
                    Duration = 2000
                });
            }
            
            _model = descriptionsResult.Value;
            _gridModel = new ObservableCollection<RegisterDescriptionEntryModel>(_model!.Entries);
            StateHasChanged();
        }
    }

    void RowRender(RowRenderEventArgs<RegisterDescriptionEntryModel> args)
    {
        args.Attributes.Add("title", "Drag row to reorder");
        args.Attributes.Add("style", "cursor:grab");
        args.Attributes.Add("draggable", "true");
        args.Attributes.Add("ondragover", "event.preventDefault();event.target.closest('.rz-data-row').classList.add('my-class')");
        args.Attributes.Add("ondragleave", "event.target.closest('.rz-data-row').classList.remove('my-class')");
        args.Attributes.Add("ondragstart", EventCallback.Factory.Create<DragEventArgs>(this, () => _draggedItem = args.Data));
        args.Attributes.Add("ondrop", EventCallback.Factory.Create<DragEventArgs>(this, () =>
        {
            var draggedIndex = _gridModel.IndexOf(_draggedItem);
            var droppedIndex = _gridModel.IndexOf(args.Data);
            _gridModel.Remove(_draggedItem);
            _gridModel.Insert(draggedIndex <= droppedIndex ? droppedIndex++ : droppedIndex, _draggedItem);
            JSRuntime.InvokeVoidAsync("eval", $"document.querySelector('.my-class').classList.remove('my-class')");
        }));
    }
    
    async Task EditRow(RegisterDescriptionEntryModel description)
    {
        if (!_grid.IsValid) return;

        // if (_editMode == DataGridEditMode.Single)
        // {
        //     Reset();
        // }

        _descriptionToUpdate.Add(description);
        await _grid.EditRow(description);
    }

    private void Up(RegisterDescriptionEntryModel entry)
    {
        var ix = _model!.Entries.IndexOf(entry);
        if (ix == 0) return;
        var newIx = ix - 1;
        
        MoveEntry(ix,newIx,entry);
        StateHasChanged();
    }

    private void Down(RegisterDescriptionEntryModel entry)
    {
        var ix = _model!.Entries.IndexOf(entry);
        if (ix >= _model.Entries.Count - 1) return;
        var newIx = ix + 1;
        
        MoveEntry(ix,newIx, entry);
        StateHasChanged();
    }
    
    private void MoveEntry(int oldIndex, int newIndex, RegisterDescriptionEntryModel entry)
    {
        _model!.Entries.RemoveAt(oldIndex);
        _model.Entries.Insert(newIndex, entry);

        foreach (var (value, index) in _model.Entries.Select((value, index) => (value, index)))
        {
            value.Order = index;
        }
    }
}