// ReSharper disable ClassNeverInstantiated.Global

namespace document.lib.rest.Api.Contract;

public record DocumentGetParameters(
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize,
    [FromQuery(Name = "unsorted")] bool? Unsorted
);

public record DocumentMoveParameters(
    [property: JsonPropertyName("folderFrom"), FromBody] int? FolderFrom,
    [property: JsonPropertyName("folderTo"), FromBody] int? FolderTo
);

public record DocumentUpdateParameters(
    [property: JsonPropertyName("name")] string? DisplayName,
    [property: JsonPropertyName("category")] string? Category,
    [property: JsonPropertyName("company")] string? Company,
    [property: JsonPropertyName("dateOfDocument")] DateTimeOffset? DateOfDocument,
    [property: JsonPropertyName("description")] string? Description
);

public record DocumentTagParameters(
    [property: JsonPropertyName("toAdd")] string[] ToAdd,
    [property: JsonPropertyName("toDelete")] string[] ToDelete
);