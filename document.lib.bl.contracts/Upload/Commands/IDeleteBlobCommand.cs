namespace document.lib.bl.contracts.Upload.Commands;

public interface IDeleteBlobCommand
{
    Task<bool> ExecuteAsync(string blobPath);
}