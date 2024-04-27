using document.lib.shared.Helper;

namespace document.lib.rest.Api;

public class DocumentApiService(IDocumentService documentService)
{
    public async Task<IResult> GetDocuments(DocumentGetQueryParameters parameters, HttpContext http)
    {
        if (PropertyValidator.Any(parameters, x => x.Id))
        {
            var document = await documentService.GetDocumentByIdAsync(parameters.Id!.Value);
            return Results.Ok(document);
        }

        if (PropertyValidator.All(parameters, x => x.Page, x => x.PageSize))
        {
            var (count, documents) = await documentService.GetDocumentsPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
            http.Response.Headers.Append("total-results", count.ToString());
            return Results.Ok(documents);
        }
        
        return Results.NotFound();
    }
}