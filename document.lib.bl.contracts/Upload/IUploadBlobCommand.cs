namespace document.lib.bl.contracts.Upload;

public interface IUploadBlobCommand
{
    Task<bool> ExecuteAsync(string blobPath, MemoryStream blob);
}