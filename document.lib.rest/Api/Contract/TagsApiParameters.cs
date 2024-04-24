namespace document.lib.rest.Api.Contract;

internal record GetTagsQueryParameters(
    [FromQuery(Name = "id")] int? Id, 
    [FromQuery(Name = "name")] string? Name,
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize);