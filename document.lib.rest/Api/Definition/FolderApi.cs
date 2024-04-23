namespace document.lib.rest.Api.Definition;

public static class FolderApi
{
    public static void AddFolderApi(this WebApplication? app)
    {
        if (app == null) return;

        app.MapGet("/folders",
                async ([AsParameters] FolderGetQueryParameters parameters, FolderApiService svc, HttpContext http) => await svc.GetFolderModel(parameters, http))
            .WithName("GetFoldersByQuery")
            .WithOpenApi();

        app.MapGet("/folders/{id}", async ([AsParameters] FolderGetRouteParameters parameters, FolderApiService svc) => await svc.GetFolderModel(parameters))
            .WithName("GetFoldersById")
            .WithOpenApi();

        app.MapPut("/folders",
                async (FolderPutParameters parameters, FolderApiService svc) => await svc.CreateFolder(parameters))
            .WithName("PutFolder")
            .WithOpenApi();

        app.MapPost("/folders/{id}",
                async ([FromQuery] int id, FolderPostParameters parameters, FolderApiService svc) => await svc.UpdateFolder(id, parameters))
            .WithName("PostFolder")
            .WithOpenApi();
    }
}