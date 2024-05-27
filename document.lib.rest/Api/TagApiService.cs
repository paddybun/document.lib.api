using document.lib.rest.Api.Constants;
using document.lib.shared.Models.Data;
using FluentValidation;

namespace document.lib.rest.Api;

internal class TagApiService(ITagService tagService, IValidator<TagsGetParameters> getValidator, IValidator<TagsUpdateParameters> updateValidator, ApiConfig config)
{
    public async Task<IResult> GetTagsAsync(TagsGetParameters parameters, HttpContext http)
    {
        var validationResult = await getValidator.ValidateAsync(parameters);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToArray());
        
        if (parameters.PageSize!.Value > config.MaxPageSize) 
            return Results.BadRequest(string.Format(ErrorMessages.PageSizeExceeded, config.MaxPageSize));

        var result = await tagService.GetTagsPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
        if (!result.IsSuccess)
        {
            return Results.StatusCode(500);
        }
        
        var (count, pagedTags) = result.Data;
        http.Response.Headers.Append("total-results", count.ToString());
        return Results.Ok(pagedTags);
    }

    public async Task<IResult> GetTagAsync(int id)
    {
        var result = await tagService.GetTagAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result.Data!) 
            : Results.NotFound();
    }

    public async Task<IResult> CreateTagsAsync(TagsUpdateParameters parameters)
    {
        var validationResult = await updateValidator.ValidateAsync(parameters);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToArray());
        
        var models = parameters.DisplayNames.Select(x => new TagModel { Name = x, DisplayName = x }).ToList();
        var result = await tagService.CreateTagsAsync(models);
        
        return result.IsSuccess 
            ? Results.Ok(result.Data) 
            : Results.StatusCode(500);
     }
}