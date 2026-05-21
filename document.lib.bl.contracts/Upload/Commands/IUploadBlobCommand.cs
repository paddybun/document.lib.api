namespace document.lib.bl.contracts.Upload.Commands;

public interface IUploadBlobCommand
{
    Task<bool> ExecuteAsync(string blobPath, MemoryStream blob);
}