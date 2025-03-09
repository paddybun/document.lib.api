namespace document.lib.shared.Cqrs.Interfaces;

public interface IUploadBlobCommand
{
    Task<bool> ExecuteAsync(string blobPath, MemoryStream blob);
}