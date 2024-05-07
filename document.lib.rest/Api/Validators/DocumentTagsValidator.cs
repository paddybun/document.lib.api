using FluentValidation;

namespace document.lib.rest.Api.Validators;

internal class DocumentTagsValidator : AbstractValidator<DocumentTagsParameters>
{
    public DocumentTagsValidator()
    {
        RuleFor(x => x.ToAdd).NotNull();
        RuleFor(x => x.ToDelete).NotNull();
    }
}