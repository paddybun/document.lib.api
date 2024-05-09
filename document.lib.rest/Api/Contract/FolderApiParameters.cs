// ReSharper disable ClassNeverInstantiated.Global

namespace document.lib.rest.Api.Contract;

internal record FolderGetQueryParameters(
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize);

internal record FolderPutParameters(
    [property: JsonPropertyName("name")] string DisplayName, 
    int DocumentsPerFolder, 
    int DocumentsPerRegister);

internal record FolderPostParameters(
    string? DisplayName, 
    int? DocumentsPerFolder, 
    int? DocumentsPerRegister);
