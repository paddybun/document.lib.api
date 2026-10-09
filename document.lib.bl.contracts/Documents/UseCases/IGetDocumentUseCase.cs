using document.lib.bl.contracts.Documents.ViewModels;
using document.lib.core;

namespace document.lib.bl.contracts.Documents.UseCases;

public interface IGetDocumentUseCase<in T> where T : IUnitOfWork
{
    Task<Result<DocumentEditModel>> ExecuteAsync(T uow, GetDocumentUseCaseParameters parameters);
}

public record GetDocumentUseCaseParameters(int DocumentId);
