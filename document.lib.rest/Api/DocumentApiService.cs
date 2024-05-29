﻿using document.lib.rest.Api.Constants;
using document.lib.shared.Helper;
using document.lib.shared.Models.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace document.lib.rest.Api;

public class DocumentApiService(ApiConfig config, IDocumentService documentService, IValidator<DocumentUpdateParameters> updateValidator, IValidator<DocumentTagsParameters> tagsValidator)
{
    public async Task<IResult> GetDocumentAsync(int id)
    {
        var result = await documentService.GetDocumentAsync(id);
        return result.IsSuccess 
            ? Results.Ok(result.Data) 
            : Results.NotFound();
    }
    
    public async Task<IResult> GetDocumentsAsync(DocumentGetQueryParameters parameters, HttpContext http)
    {
        if (parameters.PageSize.HasValue && parameters.PageSize.Value > config.MaxPageSize)
        {
            return Results.BadRequest(string.Format(ErrorMessages.PageSizeExceeded, config.MaxPageSize));
        }
        
        if (PropertyChecker.Values.All(parameters, x => x.Page, x => x.PageSize))
        {
            var pagedResult = await documentService.GetDocumentsPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
            if (pagedResult.IsSuccess)
            {
                http.Response.Headers.Append("total-results", pagedResult.Data!.Total.ToString());
                return Results.Ok(pagedResult.Data!.Results);    
            }
            return Results.NoContent();
        }

        if (PropertyChecker.Values.Any(parameters, x => x.Unsorted) && parameters.Unsorted!.Value)
        {
            int page = 0, pageSize = config.DefaultPageSize;
            if (PropertyChecker.Values.All(parameters, x => x.Page, x => x.PageSize))
            {
                page = parameters.Page!.Value;
                pageSize = parameters.PageSize!.Value;
            }
            
            var pagedResult = await documentService.GetUnsortedDocuments(page, pageSize);
            if (pagedResult.IsSuccess)
            {
                http.Response.Headers.Append("total-results", pagedResult.Data!.Total.ToString());
                return Results.Ok(pagedResult.Data!.Results);    
            }
            return Results.NoContent();
        }
        
        var sampleResult = await documentService.GetDocumentsPagedAsync(0, 10);
        if (sampleResult.IsSuccess)
        {
            return Results.Ok(sampleResult.Data!.Results);
        }
        
        return Results.NoContent();
    }
    
    public async Task<IResult> GetDocumentsForFolderAsync(string folderName, DocumentGetPagedParameters parameters, HttpContext http)
    {
        if (!PropertyChecker.Values.All(parameters, x => x.Page, x => x.PageSize))
        {
            return Results.BadRequest("Invalid parameters");
        }
        
        if (parameters.PageSize!.Value > config.MaxPageSize)
        {
            return Results.BadRequest(string.Format(ErrorMessages.PageSizeExceeded, config.MaxPageSize));
        }
        
        var result = await documentService.GetDocumentsForFolder(folderName, parameters.Page!.Value,  parameters.PageSize!.Value);
        if (result.IsSuccess)
        {
            http.Response.Headers.Append("total-results", result.Data!.Total.ToString());
            return Results.Ok(result.Data!.Results);
        }

        return Results.NoContent();
    }

    public async Task<IResult> UpdateDocumentAsync(int id, DocumentUpdateParameters parameters)
    {   
        var validationResult = await updateValidator.ValidateAsync(parameters);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToArray());

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
    
    public async Task<IResult> UpdateTagsAsync(int id, DocumentTagsParameters parameters)
    {
        var validationResult = await tagsValidator.ValidateAsync(parameters);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToArray());
        
        var document = await documentService.ModifyTagsAsync(id, parameters.ToAdd, parameters.ToDelete);
        return Results.Ok(document);
    }

    public async Task<IResult> MoveDocumentsAsync(int id, DocumentMoveParameters parameters)
    {
        if (!PropertyChecker.Values.All(parameters, x => x.FolderTo, x => x.FolderFrom))
        {
            return Results.BadRequest("Invalid parameters");
        }
        
        await documentService.MoveDocumentAsync(id, parameters.FolderFrom!.Value, parameters.FolderTo!.Value);
        return Results.Ok();
    }

    public async Task<IResult> CreateDocumentAsync(int id, DocumentUpdateParameters parameters)
    {
        var validationResult = await updateValidator.ValidateAsync(parameters);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToArray());

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