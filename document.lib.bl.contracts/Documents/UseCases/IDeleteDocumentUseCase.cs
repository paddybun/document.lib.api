using document.lib.core;

namespace document.lib.bl.contracts.Documents.UseCases;

public interface IDeleteDocumentUseCase<in T> where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, DeleteDocumentUseCaseParameters parameters);
}

public record DeleteDocumentUseCaseParameters(int DocumentId);
