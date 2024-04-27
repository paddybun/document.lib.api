namespace document.lib.rest.Api.Contract;

public record DocumentGetQueryParameters(
    [FromQuery(Name = "id")] int? Id,
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize);