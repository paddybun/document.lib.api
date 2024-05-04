using document.lib.shared.Helper;

namespace document.lib.rest.Api;

internal class TagApiService(ITagService tagService)
{
    public async Task<IResult> GetTagsAsync(GetTagsQueryParameters parameters, HttpContext http)
    {   
        if (PropertyChecker.Values.Any(parameters, x => x.Id))
        {
            var model = await tagService.GetTagByIdAsync(parameters.Id!.Value);
            return model == null 
                ? Results.NotFound() 
                : Results.Ok(model);
        }
        
        if (PropertyChecker.Values.Any(parameters, x => x.Name))
        {
            var model = await tagService.GetTagByNameAsync(parameters.Name!);
            return model == null 
                ? Results.NotFound() 
                : Results.Ok(model);
        }
        
        if (PropertyChecker.Values.All(parameters, x => x.Page, x => x.PageSize))
        {
            if (parameters.PageSize!.Value > 50) return Results.BadRequest("Max page size of 50 exceeded");
            var (count, pagedTags) = await tagService.GetTagsPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
            http.Response.Headers.Append("total-results", count.ToString());
            return Results.Ok(pagedTags);
        }
        
        var models = await tagService.GetTagsAsync();
        return Results.Ok(models);
    }

    public async Task<IResult> GetTagAsync(int id)
    {
        var model = await tagService.GetTagByIdAsync(id);
        return model == null ? Results.NotFound() : Results.Ok(model);
    }
}