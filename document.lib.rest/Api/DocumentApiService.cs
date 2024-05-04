﻿using document.lib.shared.Helper;

namespace document.lib.rest.Api;

public class DocumentApiService(IDocumentService documentService, IFolderService folderService)
{
    public async Task<IResult> GetDocumentsAsync(DocumentGetQueryParameters parameters, HttpContext http)
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
            
            var (count, documents) = await documentService.GetUnsortedDocuments(page, pageSize);
            http.Response.Headers.Append("total-results", count.ToString());
            return Results.Ok(documents);
        }
        
        return Results.NotFound();
    }

    public async Task<IResult> MoveDocumentsAsync(int id, DocumentMoveParameters parameters)
    {
        if (!PropertyChecker.Values.All(parameters, x => x.FolderTo, x => x.FolderFrom))
        {
            return Results.BadRequest("Invalid parameters");
        }
        
        await documentService.MoveDocumentAsync(id, parameters.FolderFrom!.Value, parameters.FolderTo!.Value);
        return Results.Ok();
    }
}