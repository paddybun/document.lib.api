using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace document.lib.web.v2.Components.Pages;

public partial class NewDocument : ComponentBase
{
    private record UploadedDocument(int Id, string FileName);

    private RadzenUpload _upload = null!;
    private bool _hasFile;
    private bool _uploading;
    private int _progress;
    private string _currentFileName = string.Empty;
    private readonly List<UploadedDocument> _uploadedIds = [];

    private void OnChange(UploadChangeEventArgs args)
    {
        _hasFile = args.Files.Any();
        _currentFileName = args.Files.FirstOrDefault()?.Name ?? string.Empty;
    }

    private async Task OnUpload()
    {
        _progress = 0;
        _uploading = true;
        await _upload.Upload();
    }

    private void OnProgress(UploadProgressArgs args)
    {
        _progress = args.Progress;
    }

    private async Task OnComplete(UploadCompleteEventArgs args)
    {
        _uploading = false;

        if (args.Cancelled || !TryReadId(args.RawResponse, out var id))
        {
            Notify(NotificationSeverity.Error, "The document could not be uploaded");
            return;
        }

        _uploadedIds.Insert(0, new UploadedDocument(id, _currentFileName));
        Notify(NotificationSeverity.Success, $"{_currentFileName} uploaded");

        // Clear the selection so the next file can be chosen
        await _upload.ClearFiles();
        _hasFile = false;
        _currentFileName = string.Empty;
        StateHasChanged();
    }

    private void OnError(UploadErrorEventArgs args)
    {
        _uploading = false;
        Notify(NotificationSeverity.Error, "The document could not be uploaded");
    }

    private static bool TryReadId(string? response, out int id)
    {
        id = 0;
        if (string.IsNullOrWhiteSpace(response)) return false;

        try
        {
            using var json = JsonDocument.Parse(response);
            return json.RootElement.TryGetProperty("id", out var prop) && prop.TryGetInt32(out id);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private void Notify(NotificationSeverity severity, string message) =>
        NotificationService.Notify(severity, message);
}
