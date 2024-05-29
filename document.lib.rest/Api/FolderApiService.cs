using document.lib.rest.Api.Constants;
using document.lib.shared.Helper;
using document.lib.shared.Models.Data;
using FluentValidation;

namespace document.lib.rest.Api;

internal class FolderApiService(
    IFolderService folderService, 
    IValidator<FolderUpdateParameters> updateValidator,
    ApiConfig config)
{
    public async Task<IResult> GetFolderModel(int id)
    {
        var result = await folderService.GetFolderAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result.Data) 
            : Results.NotFound(id);
    }

    public async Task<IResult> GetFolderModel(FolderGetParameters folderGetParameters, HttpContext http)
    {   
        if (!PropertyChecker.Values.All(folderGetParameters, 
                x => x.Page, 
                x => x.PageSize))
        {
            var sample = await folderService.GetFoldersPaged(0, config.DefaultPageSize);
            return sample.IsSuccess
                ? Results.Ok(sample.Data.Item2)
                : Results.StatusCode(500);    
        }

        if (folderGetParameters.PageSize >= config.MaxPageSize) 
            return Results.BadRequest(string.Format(ErrorMessages.PageSizeExceeded, config.MaxPageSize));
            
        var result = await folderService.GetFoldersPaged(folderGetParameters.Page!.Value, folderGetParameters.PageSize!.Value);
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