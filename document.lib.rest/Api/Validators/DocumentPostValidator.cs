using FluentValidation;

namespace document.lib.rest.Api.Validators;

internal class DocumentPostValidator : AbstractValidator<DocumentUpdateParameters>
{
    public DocumentPostValidator()
    {
        RuleFor(x => x.DisplayName).NotNull();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.Company).NotNull();
        RuleFor(x => x.DateOfDocument).NotNull();
        RuleFor(x => x.Description).NotNull();
    }
}