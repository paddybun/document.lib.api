using FluentValidation;

namespace document.lib.rest.Api.Validators;

internal class FolderPutValidator: AbstractValidator<FolderPutParameters>
{
    public FolderPutValidator()
    {
        RuleFor(x => x.DisplayName).NotNull().NotEmpty();
        RuleFor(x => x.DocumentsPerFolder).GreaterThan(0);
        RuleFor(x => x.DocumentsPerRegister).GreaterThan(0);
    }
}