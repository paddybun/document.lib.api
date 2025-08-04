using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface IGetRegisterUseCase
{
    Task<Result<Register>> ExecuteAsync(int folderId);
}