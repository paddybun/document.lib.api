namespace document.lib.bl.contracts.Upload;

public interface IDeleteBlobCommand
{
    Task<bool> ExecuteAsync(string blobPath);
}