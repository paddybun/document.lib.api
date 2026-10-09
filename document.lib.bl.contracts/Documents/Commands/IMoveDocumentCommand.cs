using document.lib.core;

namespace document.lib.bl.contracts.Documents.Commands;

public interface IMoveDocumentCommand<in T> where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, MoveDocumentCommandParameters parameters);
}

public record MoveDocumentCommandParameters(int DocumentId, int RegisterId, string BlobLocation, bool Digital);
