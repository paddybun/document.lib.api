using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace document.lib.rest.Parameters;

internal record FolderGetRouteParameters(
    [FromRoute(Name = "id")] int Id);

internal record FolderGetQueryParameters(
    [FromQuery(Name = "id")] int? Id, 
    [FromQuery(Name = "name")] string? Name,
    [FromQuery(Name = "page")] int? Page,
    [FromQuery(Name = "pageSize")] int? PageSize);

internal record FolderPutParameters(
    [property: JsonPropertyName("name")] string DisplayName, 
    int? DocumentsPerFolder, 
    int? DocumentsPerRegister);

internal record FolderPostParameters(
    string? DisplayName, 
    int? DocumentsPerFolder, 
    int? DocumentsPerRegister);
