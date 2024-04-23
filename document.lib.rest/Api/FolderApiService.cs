using System.Linq.Expressions;
using document.lib.rest.Api.Contract;
using document.lib.rest.Interfaces;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;

namespace document.lib.rest.Api;

internal class FolderApiService(IFolderService folderService)
{
    public async Task<FolderModel?> GetFolderModel(FolderGetRouteParameters folderGetRouteParameters)
    {
        return await folderService.GetFolderByIdAsync(folderGetRouteParameters.Id.ToString());
    }

    public async Task<IResult> GetFolderModel(FolderGetQueryParameters folderGetQueryParameters, HttpContext http)
    {
        try
        {   
            if (PropertyValidator.ValidateHasValue(folderGetQueryParameters, 
                    x => x.Page, 
                    x => x.PageSize))
            {
                var (count, folders) = await folderService.GetFoldersPaged(folderGetQueryParameters.Page!.Value, folderGetQueryParameters.PageSize!.Value);
                http.Response.Headers.Append("total-results", count.ToString());
                return Results.Ok(folders);
            }
                
            FolderModel? folder = null;
            if (PropertyValidator.ValidateHasValue(folderGetQueryParameters, x => x.Id))
                folder = await folderService.GetFolderByIdAsync(folderGetQueryParameters.Id.ToString()!);

            if (PropertyValidator.ValidateHasValue(folderGetQueryParameters, x => x.Name))
                folder = await folderService.GetFolderByNameAsync(folderGetQueryParameters.Name!);

            if (folder == null) 
                return Results.NotFound(folderGetQueryParameters);

            return Results.Ok(folder);
        }
        catch
        {
            // TODO: logging
            return Results.StatusCode(500);
        }
    }

    public async Task<IResult> CreateFolder(FolderPutParameters folderPutParameters)
    {
        try
        {
            var folder = FolderModel.New();
            folder.DocumentsFolder = folderPutParameters.DocumentsPerFolder ?? 0;
            folder.DocumentsRegister = folderPutParameters.DocumentsPerRegister ?? 0;
            folder.DisplayName = folderPutParameters.DisplayName;
            var newFolder = await folderService.CreateNewFolderAsync(folder);
            return Results.Ok(newFolder);
        }
        catch
        {
            // TODO: logging
            return Results.StatusCode(500);
        }
    }

    public async Task<IResult> UpdateFolder(int folderId, FolderPostParameters parameters)
    {
        try
        {
            if (folderId <= 0) return Results.BadRequest(folderId);

            var folder = await folderService.GetFolderByIdAsync(folderId);
            if (folder == null) return Results.NotFound(folderId);

            var hasChanged = false;

            if (!string.IsNullOrWhiteSpace(parameters.DisplayName) &&
                !folder.DisplayName!.Equals(parameters.DisplayName))
            {
                folder.DisplayName = parameters.DisplayName;
                hasChanged = true;
            }

            if (parameters.DocumentsPerFolder.HasValue && folder.DocumentsFolder != parameters.DocumentsPerFolder)
            {
                folder.DocumentsFolder = parameters.DocumentsPerFolder.Value;
                hasChanged = true;
            }

            if (parameters.DocumentsPerRegister.HasValue && folder.DocumentsRegister != parameters.DocumentsPerRegister)
            {
                folder.DocumentsRegister = parameters.DocumentsPerRegister.Value;
                hasChanged = true;
            }

            if (hasChanged) await folderService.UpdateFolderAsync(folder);
            return Results.Ok(folder);
        }
        catch
        {
            // TODO: logging
            return Results.StatusCode(500);
        }
    }
}