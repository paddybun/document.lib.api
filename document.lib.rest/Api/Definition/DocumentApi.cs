namespace document.lib.rest.Api.Definition;

public static class DocumentApi
{
    public static void UseDocumentApi(this WebApplication? app)
    {
        app?.MapGet("/documents",
                async ([AsParameters] DocumentGetParameters parameters, DocumentApiService svc,
                    HttpContext http) => await svc.GetDocumentsAsync(parameters, http))
            .WithName("GetDocumentsByQuery")
            .WithTags("Documents")
            .WithOpenApi();
        
        app?.MapGet("/documents/{id}",
                async ([FromRoute] int id, DocumentApiService svc) => await svc.GetDocumentAsync(id))
            .WithName("GetDocument")
            .WithTags("Documents")
            .WithOpenApi();
        
        app?.MapGet("documents/folder/{name}",
                async ([AsParameters] DocumentGetParameters parameters, [FromRoute] string name, DocumentApiService svc, HttpContext http) 
                    => await svc.GetDocumentsForFolderAsync(name, parameters, http))
            .WithName("GetDocumentsForFolder")
            .WithTags("Documents")
            .WithOpenApi();

        app?.MapPut("/documents/upload",
                async ([FromForm] IFormFile file, DocumentApiService svc) =>
                await svc.UploadDocumentAsync(file))
            .WithName("CreateDocument")
#if DEBUG
            .DisableAntiforgery()
#endif
            .WithTags("Documents");
        
        app?.MapPost("/documents/{id}",
                async ([FromBody] DocumentUpdateParameters parameters, [FromRoute] int id, DocumentApiService svc) =>
                await svc.UpdateDocumentAsync(id, parameters))
            .WithName("UpdateDocument")
            .WithTags("Documents")
            .WithOpenApi();

        app?.MapPost("/documents/{id}/create",
                async ([FromBody] DocumentUpdateParameters parameters, [FromRoute] int id, DocumentApiService svc) =>
                await svc.CreateDocumentAsync(id, parameters))
            .WithName("AddDocument")
            .WithTags("Documents")
            .WithOpenApi();
        
        app?.MapPost("/documents/{id}/move",
                async ([FromBody] DocumentMoveParameters parameters, [FromRoute] int id, DocumentApiService svc) =>
                await svc.MoveDocumentsAsync(id, parameters))
            .WithName("MoveDocument")
            .WithTags("Documents")
            .WithOpenApi();

        app?.MapPost("/documents/{id}/tags",
                async ([FromBody] DocumentTagParameters parameters, [FromRoute] int id, DocumentApiService svc) =>
                await svc.UpdateTagsAsync(id, parameters))
            .WithName("UpdateTags")
            .WithTags("Documents")
            .WithOpenApi();
    }
}