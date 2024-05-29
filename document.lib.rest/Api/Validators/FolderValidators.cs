using FluentValidation;

namespace document.lib.rest.Api.Validators;

internal class FolderPostValidator : AbstractValidator<FolderUpdateParameters>
{
    public FolderPostValidator()
    {
        RuleFor(x => x.DisplayName).NotNull();
        RuleFor(x => x.DocumentsPerFolder).NotNull().GreaterThan(0);
        RuleFor(x => x.DocumentsPerRegister).NotNull().GreaterThan(0);
    }
}

internal class FolderPutValidator: AbstractValidator<FolderUpdateParameters>
{
    public FolderPutValidator()
    {
        RuleFor(x => x.DisplayName).NotNull();
        RuleFor(x => x.DocumentsPerFolder).GreaterThan(0);
        RuleFor(x => x.DocumentsPerRegister).GreaterThan(0);
    }
}