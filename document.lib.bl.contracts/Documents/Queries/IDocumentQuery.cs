using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Documents.Queries;

public interface IDocumentQuery<in T> where T: IUnitOfWork
{
    Task<Result<Document>> ExecuteAsync(T uow, int id);
}