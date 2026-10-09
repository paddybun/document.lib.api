using document.lib.core;

namespace document.lib.bl.contracts.Documents.Commands;

public interface IDeleteDocumentCommand<in T> where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, DeleteDocumentCommandParameters parameters);
}

public record DeleteDocumentCommandParameters(int DocumentId);
