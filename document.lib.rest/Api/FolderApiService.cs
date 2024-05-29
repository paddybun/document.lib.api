using document.lib.shared.Models.Data;
using FluentValidation;

namespace document.lib.rest.Api;

internal sealed class FolderApiService(
    IFolderService folderService, 
    IValidator<FolderGetParameters> getValidator,
    IValidator<FolderUpdateParameters> updateValidator)
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
        if (ValidationHelper.Validate(getValidator, parameters) is { } validationResult) return validationResult;
        
        var result = await folderService.GetFoldersPaged(parameters.Page!.Value, parameters.PageSize!.Value);
        if (result.IsSuccess)
        {
            var (count, folders) = result.Data!;
            http.Response.Headers.Append("total-results", count.ToString());
            return Results.Ok(folders);
        }

        return Results.StatusCode(500);
    }

    public async Task<IResult> CreateFolder(FolderUpdateParameters parameters)
    {
        if (ValidationHelper.Validate(updateValidator, parameters) is { } validationResult) return validationResult;
        
        var newFolder = await folderService.CreateNewFolderAsync(
            parameters.DocumentsPerFolder, 
            parameters.DocumentsPerRegister, 
            parameters.DisplayName);
        return Results.Ok(newFolder);
    }

    public async Task<IResult> UpdateFolder(int folderId, FolderUpdateParameters parameters)
    {
            if (folderId <= 0) return Results.BadRequest("Parameter 'id' must be greater than 0");
            if (ValidationHelper.Validate(updateValidator, parameters) is { } validationResult) return validationResult;
            
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
}