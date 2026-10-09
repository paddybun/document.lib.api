using document.lib.core;

namespace document.lib.bl.contracts.Documents.Commands;

public interface IUpdateDocumentCommand<in T> where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, UpdateDocumentCommandParameters parameters);
}

public record UpdateDocumentCommandParameters(int DocumentId, string DisplayName, DateTimeOffset DateOfDocument, int CategoryId, string? Company, string? Description, IReadOnlyCollection<string> TagNames);
