using document.lib.shared.Models.Data;
using FluentValidation;

namespace document.lib.rest.Api;

internal sealed class CategoryApiService(
    ICategoryService categoryService, 
    IValidator<CategoryGetParameters> getValidator, 
    IValidator<CategoryUpdateParameters> updateValidator)
{
    public async Task<IResult> GetCategoryAsync(int id)
    {
        var category = await categoryService.GetCategoryAsync(id);
        return category.IsSuccess
            ? Results.Ok(category.Data)
            : Results.NotFound();
    }
    
    public async Task<IResult> GetCategoriesAsync(CategoryGetParameters parameters, HttpContext http)
    {
        if (ValidationHelper.Validate(getValidator, parameters) is { } validationResult) return validationResult;

        var categories = await categoryService.GetCategoriesPagedAsync(parameters.Page!.Value, parameters.PageSize!.Value);
        
        http.Response.Headers.Append("total-results", categories.Data!.Total.ToString());
        return categories.IsSuccess
            ? Results.Ok(categories.Data.Results)
            : Results.NotFound();
    }

    public async Task<IResult> CreateCategoryAsync(CategoryUpdateParameters parameters)
    {
        if (ValidationHelper.Validate(updateValidator, parameters) is { } validationResult) return validationResult;

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
    
    public async Task<IResult> UpdateCategoryAsync(int id, CategoryUpdateParameters parameters)
    {
        if (ValidationHelper.Validate(updateValidator, parameters) is { } validationResult) return validationResult;

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