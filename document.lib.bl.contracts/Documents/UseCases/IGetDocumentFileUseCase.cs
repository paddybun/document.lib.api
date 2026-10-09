using document.lib.bl.contracts.Documents.ViewModels;
using document.lib.core;

namespace document.lib.bl.contracts.Documents.UseCases;

public interface IGetDocumentFileUseCase<in T> where T : IUnitOfWork
{
    Task<Result<DocumentFileModel>> ExecuteAsync(T uow, GetDocumentFileUseCaseParameters parameters);
}

public record GetDocumentFileUseCaseParameters(int DocumentId);
