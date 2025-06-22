using Microsoft.EntityFrameworkCore;
using Radzen;
using System;
using Radzen.Blazor;

namespace document.lib.web.v2.Components.Pages;

public partial class NewDocument
{
    private byte[] _data;
    private RadzenUpload _upload;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private async Task OnChange(UploadChangeEventArgs arg)
    {
    }

    private async Task OnComplete(UploadCompleteEventArgs arg)
    {
    }

    private async Task OnProgress(UploadProgressArgs arg)
    {
        
    }

    private void OnError(UploadErrorEventArgs obj)
    {
        Console.WriteLine(obj.Message);
    }
}