using document.lib.shared.Models.Data;
using FluentValidation;

namespace document.lib.rest.Api;

internal sealed class DocumentApiService(
    IDocumentService documentService, 
    IValidator<DocumentGetParameters> getValidator, 
    IValidator<DocumentUpdateParameters> updateValidator, 
    IValidator<DocumentMoveParameters> moveValidator, 
    IValidator<DocumentTagParameters> tagsValidator)
{
    public async Task<IResult> GetDocumentAsync(int id)
    {
        if (id <= 0) 
            return Results.BadRequest("Parameter 'id' must be greater than 0");
        
        var result = await documentService.GetDocumentAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result.Data) 
            : Results.NotFound();
    }
    
    public async Task<IResult> GetDocumentsAsync(DocumentGetParameters parameters, HttpContext http)
    {
        if (ValidationHelper.Validate(getValidator, parameters) is { } validationResult) return validationResult;

        if (parameters.Unsorted is true)
        {
            var pagedResult = await documentService.GetUnsortedDocuments(parameters.Page!.Value, parameters.PageSize!.Value);
            if (!pagedResult.IsSuccess) return Results.NoContent();
            
            http.Response.Headers.Append("total-results", pagedResult.Data!.Total.ToString());
            return Results.Ok(pagedResult.Data!.Results);
        }
        
        var serviceResult = await documentService.GetDocumentsPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
        return serviceResult.IsSuccess ? 
            Results.Ok(serviceResult.Data!.Results) : 
            Results.NoContent();
    }
    
    public async Task<IResult> GetDocumentsForFolderAsync(string folderName, DocumentGetParameters parameters, HttpContext http)
    {
        if (ValidationHelper.Validate(getValidator, parameters) is { } validationResult) return validationResult;
        
        var result = await documentService.GetDocumentsForFolder(folderName, parameters.Page!.Value,  parameters.PageSize!.Value);
        if (!result.IsSuccess) 
            return Results.NoContent();
        
        http.Response.Headers.Append("total-results", result.Data!.Total.ToString());
        return Results.Ok(result.Data!.Results);
    }

    public async Task<IResult> UpdateDocumentAsync(int id, DocumentUpdateParameters parameters)
    {
        if (ValidationHelper.Validate(updateValidator, parameters) is { } result) return result;

        var updateModel = new DocumentModel
        {
            Id = id,
            CategoryName = parameters.Category!,
            Description = parameters.Description,
            Company = parameters.Company,
            DateOfDocument = parameters.DateOfDocument!.Value,
            DisplayName = parameters.DisplayName
        };

        var res = await documentService.UpdateDocumentAsync(updateModel);
        return Results.Ok(res);
    }
    
    public async Task<IResult> UpdateTagsAsync(int id, DocumentTagParameters parameters)
    {
        if (ValidationHelper.Validate(tagsValidator, parameters) is { } validationResult) return validationResult;
        
        var document = await documentService.ModifyTagsAsync(id, parameters.ToAdd, parameters.ToDelete);
        return Results.Ok(document);
    }

    public async Task<IResult> MoveDocumentsAsync(int id, DocumentMoveParameters parameters)
    {
        if (ValidationHelper.Validate(moveValidator, parameters) is { } validationResult) return validationResult;
                
        await documentService.MoveDocumentAsync(id, parameters.FolderFrom!.Value, parameters.FolderTo!.Value);
        return Results.Ok();
    }

    public async Task<IResult> CreateDocumentAsync(int id, DocumentUpdateParameters parameters)
    {
        if (ValidationHelper.Validate(updateValidator, parameters) is { } validationResult) return validationResult;
        
        var model = new DocumentModel
        {
            Id = id,
            CategoryName = parameters.Category!,
            DisplayName = parameters.DisplayName,
            Description = parameters.Description,
            Company = parameters.Company,
            DateOfDocument = parameters.DateOfDocument
        };
        var result = await documentService.CreateDocumentAsync(model);

        return result.IsSuccess 
            ? Results.Ok(result.Data) 
            : Results.NotFound();
    }
}