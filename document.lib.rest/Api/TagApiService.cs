namespace document.lib.rest.Api;

internal class TagApiService(ITagService tagService)
{
    public async Task<IResult> GetTagsAsync(GetTagsQueryParameters parameters, HttpContext http)
    {
        var models = await tagService.GetTagsAsync();
        return Results.Ok(models);
    }
}