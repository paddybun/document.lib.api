namespace document.lib.rest.Api.Contract;

internal record GetCategoryParams (
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize);

internal record UpdateCategoryParams(
    [property: JsonPropertyName("displayName")] string? DisplayName,
    [property: JsonPropertyName("description")] string? Description);