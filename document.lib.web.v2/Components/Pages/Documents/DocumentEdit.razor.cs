using document.lib.bl.contracts.Categories.UseCases;
using document.lib.bl.contracts.Documents.UseCases;
using document.lib.bl.contracts.Documents.ViewModels;
using document.lib.bl.shared;
using document.lib.data.entities;
using document.lib.data.models.Documents;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace document.lib.web.v2.Components.Pages.Documents;

public partial class DocumentEdit : ComponentBase
{
    [Parameter] public int Id { get; set; }

    private DocumentEditModel? _model;
    private List<Category> _categories = [];
    private List<Tag> _tags = [];
    private bool _saving;
    private bool _showPreview;
    private string _newTag = string.Empty;
    private int _nextTempId = -1;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);

        var docResult = await GetDocumentUseCase.ExecuteAsync(uow, new GetDocumentUseCaseParameters(Id));
        var categories = await GetCategoriesUseCase.ExecuteAsync(uow, new GetCategoriesUseCaseParameters(null, null));
        var tags = await GetTagsUseCase.ExecuteAsync(uow);

        if (!docResult.HasData || !categories.IsSuccess || !tags.IsSuccess)
        {
            Notify(NotificationSeverity.Error, "Document could not be loaded");
            return;
        }

        _categories = categories.Value!.Where(x => x.Name != Constants.UncategorizedName).ToList();
        _tags = tags.Value!.ToList();
        _model = docResult.Value;
        StateHasChanged();
    }

    private async Task Save()
    {
        if (_model is not { DateOfDocument: { } dateOfDocument, CategoryId: { } categoryId }) return;

        var tagNames = _tags
            .Where(x => _model.TagIds.Contains(x.Id))
            .Select(x => x.Id > 0 ? x.Name : x.DisplayName ?? x.Name)
            .ToList();

        _saving = true;
        using var uow = await UnitOfWork.CreateAsync(DbContextFactory);
        var result = await SaveDocumentUseCase.ExecuteAsync(uow, new SaveDocumentUseCaseParameters(_model.Id,
            new DocumentSaveModel
            {
                DisplayName = _model.DisplayName ?? string.Empty,
                DateOfDocument = dateOfDocument,
                CategoryId = categoryId,
                Tags = tagNames
            }));
        _saving = false;

        if (result.IsSuccess)
        {
            Notify(NotificationSeverity.Success, "Document saved");
            await ReloadAsync();
        }
        else
            Notify(NotificationSeverity.Error, "Document could not be saved");
    }

    private void AddTag()
    {
        var name = _newTag.Trim();
        if (_model == null || name.Length == 0) return;

        var tag = _tags.FirstOrDefault(x => string.Equals(x.DisplayName ?? x.Name, name, StringComparison.OrdinalIgnoreCase)
                                            || string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
                  ?? new Tag { Id = _nextTempId--, Name = name.ToLowerInvariant(), DisplayName = name };
        if (!_tags.Contains(tag)) _tags.Add(tag);

        _model.TagIds = _model.TagIds.Append(tag.Id).Distinct().ToList();
        _newTag = string.Empty;
    }

    private void Back() => NavigationManager.NavigateTo("/Documents");

    private void Notify(NotificationSeverity severity, string message) =>
        NotificationService.Notify(severity, message);
}
