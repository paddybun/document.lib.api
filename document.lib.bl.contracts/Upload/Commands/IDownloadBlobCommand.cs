namespace document.lib.bl.contracts.Upload.Commands;

public interface IDownloadBlobCommand
{
    Task<Stream?> ExecuteAsync(string blobPath);
}
