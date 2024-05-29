// ReSharper disable ClassNeverInstantiated.Global
namespace document.lib.rest.Api.Contract;

internal record TagsGetParameters (
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize);
    
internal record TagsUpdateParameters(
    [property: JsonPropertyName("names")] string[] DisplayNames);