using document.lib.core;

namespace document.lib.bl.contracts.Documents.UseCases;

public interface IGetCompaniesUseCase<in T> where T : IUnitOfWork
{
    Task<Result<List<string>>> ExecuteAsync(T uow);
}
