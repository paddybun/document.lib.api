using FluentValidation;

namespace document.lib.rest.Api.Validators;

internal class TagGetValidator: AbstractValidator<TagsGetParameters>
{
    public TagGetValidator(ApiConfig config)
    {
        RuleFor(x => x.Page).NotNull().InclusiveBetween(0, int.MaxValue);
        RuleFor(x => x.PageSize).NotNull().InclusiveBetween(1, config.MaxPageSize);
    }
}

internal class TagUpdateValidator: AbstractValidator<TagsUpdateParameters>
{
    public TagUpdateValidator()
    {
        RuleFor(x => x.DisplayNames).NotEmpty();
        RuleForEach(x => x.DisplayNames).NotEmpty();
    }
}