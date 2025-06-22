using document.lib.data.entities;

namespace document.lib.bl.contracts.Upload;

public interface IUploadBlobUseCase
{
    Task<Document?> ExecuteAsync(string filename, MemoryStream blob);
}