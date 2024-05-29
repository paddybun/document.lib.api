// ReSharper disable ClassNeverInstantiated.Global

namespace document.lib.rest.Api.Contract;

internal record FolderGetParameters(
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize);

internal record FolderUpdateParameters(
    [property: JsonPropertyName("name")] string DisplayName, 
    int DocumentsPerFolder, 
    int DocumentsPerRegister);
