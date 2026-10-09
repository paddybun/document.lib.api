using document.lib.bl.contracts.Documents.Commands;
using document.lib.core;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Documents.Commands;

public class DeleteDocumentCommand : IDeleteDocumentCommand<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, DeleteDocumentCommandParameters parameters)
    {
        var doc = await uow.Connection.Documents
            .Include(x => x.Register)
            .SingleOrDefaultAsync(x => x.Id == parameters.DocumentId);

        if (doc == null)
            return Result<int>.Warning("Document not found");

        // Unsorted documents were never counted in a register
        if (!doc.Unsorted && doc.Register.DocumentCount > 0)
            doc.Register.DocumentCount--;

        // Tag assignments are removed by cascade
        uow.Connection.Documents.Remove(doc);

        return Result<int>.Success(doc.Id);
    }
}
