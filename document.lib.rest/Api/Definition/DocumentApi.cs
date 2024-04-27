namespace document.lib.rest.Api.Definition;

public static class DocumentApi
{
    public static void AddDocumentApi(this WebApplication? app)
    {
        app?.MapGet("/documents",
                async ([AsParameters] DocumentGetQueryParameters parameters, DocumentApiService svc, HttpContext http) => await svc.GetDocuments(parameters, http))
            .WithName("GetDocumentsByQuery")
            .WithOpenApi();

        // app?.MapGet("/documents/{id}", async ([AsParameters] DocumentGetRouteParameters parameters, DocumentApiService svc) => await svc.GetDocumentModel(parameters))
        //     .WithName("GetDocumentsById")
        //     .WithOpenApi();
        //
        // app?.MapPut("/documents",
        //         async (DocumentPutParameters parameters, DocumentApiService svc) => await svc.CreateDocument(parameters))
        //     .WithName("PutDocument")
        //     .WithOpenApi();
        //
        // app?.MapPost("/documents/{id}",
        //         async ([FromQuery] int id, DocumentPostParameters parameters, DocumentApiService svc) => await svc.UpdateDocument(id, parameters))
        //     .WithName("PostDocument")
        //     .WithOpenApi();
    }
}