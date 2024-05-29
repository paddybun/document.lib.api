using document.lib.rest.Api.Constants;
using document.lib.shared.Models.Data;
using FluentValidation;

namespace document.lib.rest.Api;

internal sealed class TagApiService(
    ITagService tagService, 
    IValidator<TagsGetParameters> getValidator, 
    IValidator<TagsUpdateParameters> updateValidator)
{
    public async Task<IResult> GetTagAsync(int id)
    {
        if (id <= 0) 
            return Results.BadRequest("Parameter 'id' must be greater than 0");
        
        var result = await tagService.GetTagAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result.Data!) 
            : Results.NotFound();
    }

    public async Task<IResult> GetTagsAsync(TagsGetParameters parameters, HttpContext http)
    {
        if (ValidationHelper.Validate(getValidator, parameters) is { } validationResult) return validationResult;
        
        var result = await tagService.GetTagsPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
        if (!result.IsSuccess) 
            return Results.StatusCode(500);
        
        var (count, pagedTags) = result.Data;
        http.Response.Headers.Append("total-results", count.ToString());
        return Results.Ok(pagedTags);
    }

    public async Task<IResult> CreateTagsAsync(TagsUpdateParameters parameters)
    {
        if (ValidationHelper.Validate(updateValidator, parameters) is { } validationResult) return validationResult;
        
        var models = parameters.DisplayNames.Select(x => new TagModel { Name = x, DisplayName = x }).ToList();
        var result = await tagService.CreateTagsAsync(models);
        
        return result.IsSuccess 
            ? Results.Ok(result.Data) 
            : Results.StatusCode(500);
     }
}