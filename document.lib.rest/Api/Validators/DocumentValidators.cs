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

internal class DocumentTagsValidator : AbstractValidator<DocumentTagsParameters>
{
    public DocumentTagsValidator()
    {
        RuleFor(x => x.ToAdd).NotNull();
        RuleFor(x => x.ToDelete).NotNull();
    }
}