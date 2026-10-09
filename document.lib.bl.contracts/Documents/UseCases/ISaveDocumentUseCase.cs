using document.lib.core;
using document.lib.data.entities;
using document.lib.data.models.Documents;

namespace document.lib.bl.contracts.Documents.UseCases;

public interface ISaveDocumentUseCase<in T>
    where T: IUnitOfWork
{
    Task<Result<Document>> ExecuteAsync(T uow, SaveDocumentUseCaseParameters parameters);
}

public record SaveDocumentUseCaseParameters(int DocumentId, DocumentSaveModel Document);
