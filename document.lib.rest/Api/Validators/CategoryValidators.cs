using FluentValidation;

namespace document.lib.rest.Api.Validators;

internal class CategoryGetValidator : AbstractValidator<GetCategoryParams>
{
    public CategoryGetValidator(ApiConfig config)
    {
        RuleFor(x => x.Page).NotNull().InclusiveBetween(0, int.MaxValue).WithName("page");
        RuleFor(x => x.PageSize).NotNull().InclusiveBetween(1, config.MaxPageSize).WithName("pageSize");
    }
}

internal class CategoryUpdateValidator : AbstractValidator<UpdateCategoryParams>
{
    public CategoryUpdateValidator()
    {
        RuleFor(x => x.DisplayName).MaximumLength(500);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}