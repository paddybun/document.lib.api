using document.lib.core;

namespace document.lib.bl.contracts.Documents.Queries;

public interface ICompaniesQuery<in T> where T : IUnitOfWork
{
    Task<Result<List<string>>> ExecuteAsync(T uow);
}
