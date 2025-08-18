using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.DocumentHandling;

public interface IGetRegisterUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<Register>> ExecuteAsync(T uow, GetRegisterUseCaseParameters parameters);
}

public record GetRegisterUseCaseParameters(int FolderId);