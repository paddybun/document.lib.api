namespace document.lib.rest.Api.Definition;

public static class FolderApi
{
    public static void UseFolderApi(this WebApplication? app)
    {
        app?.MapGet("/folders",
                async ([AsParameters] FolderGetParameters parameters, FolderApiService svc, HttpContext http) => await svc.GetFolderModel(parameters, http))
            .WithName("GetFolders")
            .WithTags("Folders")
            .WithOpenApi();

        app?.MapGet("/folders/{id}", async (int id, FolderApiService svc) => await svc.GetFolderModel(id))
            .WithName("GetFolder")
            .WithTags("Folders")
            .WithOpenApi();

        app?.MapPut("/folders",
                async ([FromBody]FolderUpdateParameters parameters, FolderApiService svc) => await svc.CreateFolder(parameters))
            .WithName("CreateFolder")
            .WithTags("Folders")
            .WithOpenApi();

        app?.MapPost("/folders/{id}",
                async ([FromQuery] int id, FolderUpdateParameters parameters, FolderApiService svc) => await svc.UpdateFolder(id, parameters))
            .WithName("UpdateFolder")
            .WithTags("Folders")
            .WithOpenApi();
    }
}