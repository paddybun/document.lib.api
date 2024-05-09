using document.lib.rest.Api.Constants;
using document.lib.shared.Helper;
using document.lib.shared.Models.Data;
using FluentValidation;

namespace document.lib.rest.Api;

internal class FolderApiService(
    IFolderService folderService, 
    IValidator<FolderPutParameters> folderPutValidator, 
    IValidator<FolderPostParameters> folderPostValidator,
    ApiConfig config)
{
    public async Task<IResult> GetFolderModel(int id)
    {
        var result = await folderService.GetFolderAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result.Data) 
            : Results.NotFound(id);
    }

    public async Task<IResult> GetFolderModel(FolderGetQueryParameters folderGetQueryParameters, HttpContext http)
    {   
        if (!PropertyChecker.Values.All(folderGetQueryParameters, 
                x => x.Page, 
                x => x.PageSize))
        {
            var sample = await folderService.GetFoldersPaged(0, config.DefaultPageSize);
            return sample.IsSuccess
                ? Results.Ok(sample.Data.Item2)
                : Results.StatusCode(500);    
        }

        if (folderGetQueryParameters.PageSize >= config.MaxPageSize) 
            return Results.BadRequest(string.Format(ErrorMessages.PageSizeExceeded, config.MaxPageSize));
            
        var result = await folderService.GetFoldersPaged(folderGetQueryParameters.Page!.Value, folderGetQueryParameters.PageSize!.Value);
        if (result.IsSuccess)
        {
            var (count, folders) = result.Data;
            http.Response.Headers.Append("total-results", count.ToString());
            return Results.Ok(folders);
        }

        return Results.StatusCode(500);
    }

    public async Task<IResult> CreateFolder(FolderPutParameters folderPutParameters)
    {
        try
        {
            var res = await folderPutValidator.ValidateAsync(folderPutParameters);
            if (!res.IsValid)
            {
                var validationErrors = res.Errors
                    .GroupBy(x => x.PropertyName)
                    .Select(g => new ValidationError(g.Key, string.Join(" | ", g.Select(x => x.ErrorMessage)))).ToList();
                return Results.BadRequest(validationErrors);
            }
            
            var newFolder = await folderService.CreateNewFolderAsync(
                folderPutParameters.DocumentsPerFolder, 
                folderPutParameters.DocumentsPerRegister, 
                folderPutParameters.DisplayName);
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
            var validationResults = await folderPostValidator.ValidateAsync(parameters);
            if (!validationResults.IsValid)
            {
                var validationErrors = validationResults.Errors.Select(x => new ValidationError(x.PropertyName,x.ErrorMessage)).ToList();
                return Results.BadRequest(validationErrors);
            }

            var folderResult = await folderService.UpdateFolderAsync(new FolderModel
            {
                Id = folderId,
                DisplayName = parameters.DisplayName,
                DocumentsFolder = parameters.DocumentsPerFolder!.Value,
                DocumentsRegister = parameters.DocumentsPerRegister!.Value
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