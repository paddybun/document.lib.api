using document.lib.rest.Api.Constants;
using document.lib.shared.Helper;
using document.lib.shared.Models.Data;
using FluentValidation;

namespace document.lib.rest.Api;

internal class FolderApiService(
    IFolderService folderService, 
    IValidator<FolderGetParameters> getValidator,
    IValidator<FolderUpdateParameters> updateValidator,
    ApiConfig config)
{
    public async Task<IResult> GetFolder(int id)
    {
        var result = await folderService.GetFolderAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result.Data) 
            : Results.NotFound(id);
    }

    public async Task<IResult> GetFolders(FolderGetParameters parameters, HttpContext http)
    {   
        var validationResult = await getValidator.ValidateAsync(parameters);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToArray());
            
        var result = await folderService.GetFoldersPaged(parameters.Page!.Value, parameters.PageSize!.Value);
        if (result.IsSuccess)
        {
            var (count, folders) = result.Data;
            http.Response.Headers.Append("total-results", count.ToString());
            return Results.Ok(folders);
        }

        return Results.StatusCode(500);
    }

    public async Task<IResult> CreateFolder(FolderUpdateParameters parameters)
    {
        try
        {   
            var validationResult = await updateValidator.ValidateAsync(parameters);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToArray());
            
            var newFolder = await folderService.CreateNewFolderAsync(
                parameters.DocumentsPerFolder, 
                parameters.DocumentsPerRegister, 
                parameters.DisplayName);
            return Results.Ok(newFolder);
        }
        catch
        {
            // TODO: logging
            return Results.StatusCode(500);
        }
    }

    public async Task<IResult> UpdateFolder(int folderId, FolderUpdateParameters parameters)
    {
        try
        {
            if (folderId <= 0) return Results.BadRequest(folderId);
            
            var validationResult = await updateValidator.ValidateAsync(parameters);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToArray());
            
            var folderResult = await folderService.UpdateFolderAsync(new FolderModel
            {
                Id = folderId,
                DisplayName = parameters.DisplayName,
                DocumentsFolder = parameters.DocumentsPerFolder,
                DocumentsRegister = parameters.DocumentsPerRegister
            });
            
            return folderResult.IsSuccess
                ? Results.Ok(folderResult.Data)
                : Results.NotFound(folderId);
        }
        catch
        {
            // TODO: logging
            return Results.StatusCode(500);
        }
    }
}