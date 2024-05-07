using document.lib.rest.Api.Constants;
using document.lib.shared.Helper;
using FluentValidation;

namespace document.lib.rest.Api;

public class DocumentApiService(ApiConfig config, IDocumentService documentService, IValidator<DocumentUpdateParameters> documentPostValidator, IValidator<DocumentTagsParameters> documentTagsValidator)
{
    public async Task<IResult> GetDocumentAsync(int id)
    {
        var document = await documentService.GetDocumentByIdAsync(id);
        return document is null ? Results.NotFound() : Results.Ok(document);
    }
    
    public async Task<IResult> GetDocumentsAsync(DocumentGetQueryParameters parameters, HttpContext http)
    {
        if (PropertyChecker.Values.All(parameters, x => x.Page, x => x.PageSize))
        {
            if (parameters.PageSize!.Value > 50)
            {
                return Results.BadRequest(string.Format(ErrorMessages.PageSizeExceeded, config.MaxPageSize));
            }

            var (count, documents) = await documentService.GetDocumentsPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
            http.Response.Headers.Append("total-results", count.ToString());
            return Results.Ok(documents);
        }

        if (PropertyChecker.Values.Any(parameters, x => x.Unsorted) && parameters.Unsorted!.Value)
        {
            int page = 0, pageSize = int.MaxValue;
            if (PropertyChecker.Values.All(parameters, x => x.Page, x => x.PageSize))
            {
                page = parameters.Page!.Value;
                pageSize = parameters.PageSize!.Value;
            }
            
            var (count, documents) = await documentService.GetUnsortedDocuments(page, pageSize);
            http.Response.Headers.Append("total-results", count.ToString());
            return Results.Ok(documents);
        }
        
        
        var (_, docs) = await documentService.GetDocumentsPagedAsync(0, 100);
        return Results.Ok(docs);
    }

    public async Task<IResult> UpdateDocumentAsync(int id, DocumentUpdateParameters parameters)
    {
        var validationResults = await documentPostValidator.ValidateAsync(parameters);
        if (!validationResults.IsValid)
        {
            var validationErrors = validationResults.Errors.Select(x => new ValidationError(x.PropertyName,x.ErrorMessage)).ToList();
            return Results.BadRequest(validationErrors);
        }

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
        var validationResults = await documentTagsValidator.ValidateAsync(parameters);
        if (!validationResults.IsValid)
        {
            var validationErrors = validationResults.Errors.Select(x => new ValidationError(x.PropertyName,x.ErrorMessage)).ToList();
            return Results.BadRequest(validationErrors);
        }
        
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
}