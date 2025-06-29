using document.lib.bl.contracts.Documents;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Documents;

public class SaveDocumentUseCase(
    ILogger<SaveDocumentUseCase> logger,
    DatabaseContext context): ISaveDocumentUseCase
{
    public async Task<Result<Document>> ExecuteAsync(Document document)
    {
        try
        {
            return Result<Document>.Failure();
        }
        catch (Exception ex)
        {
            return Result<Document>.Failure();
        }
    }
}