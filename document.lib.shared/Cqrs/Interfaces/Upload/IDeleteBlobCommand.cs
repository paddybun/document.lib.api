namespace document.lib.shared.Cqrs.Interfaces.Upload;

public interface IDeleteBlobCommand
{
    Task<bool> ExecuteAsync(string blobPath);
}