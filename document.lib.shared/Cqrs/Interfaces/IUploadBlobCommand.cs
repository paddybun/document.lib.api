namespace document.lib.shared.Cqrs.Interfaces;

public interface IUploadBlobCommand
{
    Task<bool> ExecuteAsync(string name, MemoryStream blob);
}