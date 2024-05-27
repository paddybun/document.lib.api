namespace document.lib.rest.Api.Contract;

internal record CategoryGetParams (
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize);

internal record CategoryUpdateParams(
    [property: JsonPropertyName("displayName")] string? DisplayName,
    [property: JsonPropertyName("description")] string? Description);