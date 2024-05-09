using document.lib.rest.Api.Constants;
using document.lib.shared.Helper;

namespace document.lib.rest.Api;

internal class TagApiService(ITagService tagService, ApiConfig config)
{
    public async Task<IResult> GetTagsAsync(GetTagsQueryParameters parameters, HttpContext http)
    {   
        if (PropertyChecker.Values.Any(parameters, x => x.Id))
        {
            var result = await tagService.GetTagByIdAsync(parameters.Id!.Value);
            return result.IsSuccess
                ? Results.Ok(result) 
                : Results.NotFound();
        }
        
        if (PropertyChecker.Values.Any(parameters, x => x.Name))
        {
            var result = await tagService.GetTagByNameAsync(parameters.Name!);
            return result.IsSuccess 
                ? Results.Ok(result) 
                : Results.NotFound();
        }
        
        if (PropertyChecker.Values.All(parameters, x => x.Page, x => x.PageSize))
        {
            if (parameters.PageSize!.Value > config.MaxPageSize) 
                return Results.BadRequest(string.Format(ErrorMessages.PageSizeExceeded, config.MaxPageSize));

            var result = await tagService.GetTagsPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
            if (result.IsSuccess)
            {
                var (count, pagedTags) = result.Data;
                http.Response.Headers.Append("total-results", count.ToString());
                return Results.Ok(pagedTags);
            }
            return Results.StatusCode(500);
        }
        
        var models = await tagService.GetTagsAsync();
        return Results.Ok(models);
    }

    public async Task<IResult> GetTagAsync(int id)
    {
        var result = await tagService.GetTagByIdAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result) 
            : Results.NotFound();
    }
}