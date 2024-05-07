namespace document.lib.rest.Api.Definition;

public static class DocumentApi
{
    public static void UseDocumentApi(this WebApplication? app)
    {
        app?.MapGet("/documents",
                async ([AsParameters] DocumentGetQueryParameters parameters, DocumentApiService svc,
                    HttpContext http) => await svc.GetDocumentsAsync(parameters, http))
            .WithName("GetDocumentsByQuery")
            .WithOpenApi();
        
        app?.MapGet("/documents/{id}",
                async ([FromRoute] int id, DocumentApiService svc) => await svc.GetDocumentAsync(id))
            .WithName("GetDocument")
            .WithOpenApi();

        app?.MapPost("/documents/{id}",
                async ([FromBody] DocumentUpdateParameters parameters, [FromRoute] int id, DocumentApiService svc) =>
                await svc.UpdateDocumentAsync(id, parameters))
            .WithName("UpdateDocument")
            .WithOpenApi();

        app?.MapPost("/documents/{id}/move",
                async ([FromBody] DocumentMoveParameters parameters, [FromRoute] int id, DocumentApiService svc) =>
                await svc.MoveDocumentsAsync(id, parameters))
            .WithName("MoveDocument")
            .WithOpenApi();

        app?.MapPost("/documents/{id}/tags",
                async ([FromBody] DocumentTagsParameters parameters, [FromRoute] int id, DocumentApiService svc) =>
                await svc.UpdateTagsAsync(id, parameters))
            .WithName("UpdateTags")
            .WithOpenApi();
    }
}