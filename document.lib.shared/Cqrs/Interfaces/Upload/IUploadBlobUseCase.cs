using document.lib.ef.Entities;

namespace document.lib.shared.Cqrs.Interfaces;

public interface IUploadBlobUseCase
{
    Task<EfDocument?> ExecuteAsync(string filename, MemoryStream blob);
}