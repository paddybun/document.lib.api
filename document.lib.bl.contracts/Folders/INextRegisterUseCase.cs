using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface INextRegisterUseCase
{
    Task<Result<Register>> ExecuteAsync(int folderId);
}