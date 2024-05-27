namespace document.lib.rest.Api.Definition;

public static class CategoryApi
{
    public static void UseCatergoryApi(this WebApplication? app)
    {
        app?.MapGet("/categories", async ([AsParameters]CategoryGetParams parameters, CategoryApiService svc, HttpContext http) => await svc.GetCategoriesAsync(parameters, http))
            .WithName("GetCategories")
            .WithTags("Category")
            .WithOpenApi();
        
        app?.MapGet("/categories/{id}", async ([FromRoute] int id, CategoryApiService svc) => await svc.GetCategoryAsync(id))
            .WithName("GetCategory")
            .WithTags("Category")
            .WithOpenApi();
        
        app?.MapPut("/categories", async ([FromBody]CategoryUpdateParams parameters, CategoryApiService svc) => await svc.CreateCategoryAsync(parameters))
            .WithName("CreateCategory")
            .WithTags("Category")
            .WithOpenApi();
        
        app?.MapPost("/categories/{id}", async ([FromRoute] int id, [FromBody]CategoryUpdateParams parameters, CategoryApiService svc) => await svc.UpdateCategoryAsync(id, parameters))
            .WithName("UpdateCategory")
            .WithTags("Category")
            .WithOpenApi();
    }
}