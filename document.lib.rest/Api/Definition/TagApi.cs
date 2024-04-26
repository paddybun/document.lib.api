namespace document.lib.rest.Api.Definition;

public static class TagApi
{
    public static void AddTagApi(this WebApplication? app)
    {
        app?.MapGet("/tags/{id}", async ([FromRoute] int id, TagApiService svc) => await svc.GetTagAsync(id));
            
        app?.MapGet("/tags", async ([AsParameters]GetTagsQueryParameters parameters, TagApiService svc, HttpContext http) => await svc.GetTagsAsync(parameters, http))
            .WithName("GetTags")
            .WithOpenApi();
    }
}