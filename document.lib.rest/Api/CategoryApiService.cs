using System.Text;
using document.lib.shared.Models.Data;
using FluentValidation;

namespace document.lib.rest.Api;

internal sealed class CategoryApiService(ICategoryService categoryService, IValidator<CategoryGetParams> getValidator, IValidator<CategoryUpdateParams> updateValidator)
{
    public async Task<IResult> GetCategoryAsync(int id)
    {
        var category = await categoryService.GetCategoryAsync(id);
        return category.IsSuccess
            ? Results.Ok(category.Data)
            : Results.NotFound();
    }
    
    public async Task<IResult> GetCategoriesAsync(CategoryGetParams parameters, HttpContext http)
    {
        var validationResult = await getValidator.ValidateAsync(parameters);
        if (!validationResult.IsValid)
        {
            var message = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
            return Results.BadRequest(message);
        }

        var categories = await categoryService.GetCategoriesPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
        if (!categories.IsSuccess)
        {
            return Results.StatusCode(500);
        }
        
        http.Response.Headers.Append("total-results", categories.Data!.Total.ToString());
        return categories.IsSuccess
            ? Results.Ok(categories.Data.Results)
            : Results.NotFound();
    }

    public async Task<IResult> CreateCategoryAsync(CategoryUpdateParams parameters)
    {
        var validationResult = await updateValidator.ValidateAsync(parameters);
        if (!validationResult.IsValid)
        {
            var message = string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage));
            return Results.BadRequest(message);
        }

        var updateModel = new CategoryModel
        {
            DisplayName = parameters.DisplayName,
            Description = parameters.Description
        };
        var category = await categoryService.CreateCategoryAsync(updateModel);
        return category.IsSuccess
            ? Results.Ok(category.Data!)
            : Results.StatusCode(500);
    }
    
    public async Task<IResult> UpdateCategoryAsync(int id, CategoryUpdateParams parameters)
    {
        var validationResult = await updateValidator.ValidateAsync(parameters);
        if (!validationResult.IsValid)
        {
            var message = string.Join(Environment.NewLine, validationResult.Errors.Select(x => x.ErrorMessage));
            return Results.BadRequest(message);
        }

        var updateModel = new CategoryModel
        {
            Id = id,
            DisplayName = parameters.DisplayName,
            Description = parameters.Description
        };
        var category = await categoryService.UpdateCategory(updateModel);
        return category.IsSuccess
            ? Results.Ok(category.Data!)
            : Results.StatusCode(500);
    }
}