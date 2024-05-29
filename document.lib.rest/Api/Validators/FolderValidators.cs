using FluentValidation;

namespace document.lib.rest.Api.Validators;

internal class FolderGetValidator : AbstractValidator<FolderGetParameters>
{
    public FolderGetValidator(ApiConfig config)
    {
        RuleFor(x => x.Page).NotNull().InclusiveBetween(0, int.MaxValue).WithName("page");
        RuleFor(x => x.PageSize).NotNull().InclusiveBetween(1, config.MaxPageSize).WithName("pageSize");
    }
}

internal class FolderUpdateValidator: AbstractValidator<FolderUpdateParameters>
{
    public FolderUpdateValidator()
    {
        RuleFor(x => x.DisplayName).NotNull();
        RuleFor(x => x.DocumentsPerFolder).GreaterThan(0);
        RuleFor(x => x.DocumentsPerRegister).GreaterThan(0);
    }
}