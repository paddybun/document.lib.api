using document.lib.bl.contracts.Documents.Commands;
using document.lib.core;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Documents.Commands;

public class MoveDocumentCommand : IMoveDocumentCommand<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, MoveDocumentCommandParameters parameters)
    {
        var doc = await uow.Connection.Documents.SingleOrDefaultAsync(x => x.Id == parameters.DocumentId);
        if (doc == null)
            return Result<int>.Warning("Document not found");

        var register = await uow.Connection.Registers.SingleOrDefaultAsync(x => x.Id == parameters.RegisterId);
        if (register == null)
            return Result<int>.Warning("Register not found");

        // The unsorted register does not track its document count
        register.DocumentCount++;
        doc.Register = register;
        doc.BlobLocation = parameters.BlobLocation;
        doc.Unsorted = false;
        doc.Digital = parameters.Digital;

        return Result<int>.Success(doc.Id);
    }
}
