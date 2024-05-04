// ReSharper disable ClassNeverInstantiated.Global
namespace document.lib.rest.Api.Contract;

public record DocumentGetQueryParameters(
    [FromQuery(Name = "id")] int? Id,
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize,
    [FromQuery(Name = "unsorted")] bool? Unsorted
);

public record DocumentMoveParameters(
    [property: JsonPropertyName("folderFrom"), FromBody] int? FolderFrom,
    [property: JsonPropertyName("folderTo"), FromBody] int? FolderTo
);