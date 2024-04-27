using document.lib.shared.Helper;

namespace document.lib.rest.Api;

public class DocumentApiService(IDocumentService documentService)
{
    public async Task<IResult> GetDocuments(DocumentGetQueryParameters parameters, HttpContext http)
    {
        if (PropertyChecker.Values.Any(parameters, x => x.Id))
        {
            var document = await documentService.GetDocumentByIdAsync(parameters.Id!.Value);
            return Results.Ok(document);
        }

        if (PropertyChecker.Values.All(parameters, x => x.Page, x => x.PageSize))
        {
            var (count, documents) = await documentService.GetDocumentsPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
            http.Response.Headers.Append("total-results", count.ToString());
            return Results.Ok(documents);
        }

        if (PropertyChecker.Values.Any(parameters, x => x.Unsorted) && parameters.Unsorted!.Value)
        {
            int page = 0, pageSize = int.MaxValue;
            if (PropertyChecker.Values.All(parameters, x => x.Page, x => x.PageSize))
            {
                page = parameters.Page!.Value;
                pageSize = parameters.PageSize!.Value;
            }
            var documents = await documentService.GetUnsortedDocuments(page, pageSize);
            return Results.Ok(documents);
        }
        
        return Results.NotFound();
    }
}