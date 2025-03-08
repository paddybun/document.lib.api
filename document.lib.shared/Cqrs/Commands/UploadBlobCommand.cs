using document.lib.shared.Cqrs.Interfaces;

namespace document.lib.shared.Cqrs.Commands;

public class UploadBlobCommand(): IUploadBlobCommand
{
    public Task<bool> ExecuteAsync(string name, MemoryStream blob)
    {
        throw new NotImplementedException();
    }
}