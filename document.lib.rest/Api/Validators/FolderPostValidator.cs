using FluentValidation;

namespace document.lib.rest.Api.Validators;

internal class FolderPostValidator : AbstractValidator<FolderPostParameters>
{
    public FolderPostValidator()
    {
        RuleFor(x => x.DisplayName).NotNull();
        RuleFor(x => x.DocumentsPerFolder).NotNull().GreaterThan(0);
        RuleFor(x => x.DocumentsPerRegister).NotNull().GreaterThan(0);
    }
}