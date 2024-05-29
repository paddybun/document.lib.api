// ReSharper disable ClassNeverInstantiated.Global
namespace document.lib.rest.Api.Contract;

internal record CategoryGetParameters (
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize);

internal record CategoryUpdateParameters(
    [property: JsonPropertyName("displayName")] string? DisplayName,
    [property: JsonPropertyName("description")] string? Description);