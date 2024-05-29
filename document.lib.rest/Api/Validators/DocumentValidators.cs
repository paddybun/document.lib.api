using FluentValidation;

namespace document.lib.rest.Api.Validators;

internal class DocumentGetValidator : AbstractValidator<DocumentGetParameters>
{
    public DocumentGetValidator(ApiConfig config)
    {
        RuleFor(x => x.Page).NotNull().InclusiveBetween(0, int.MaxValue).WithName("page");
        RuleFor(x => x.PageSize).NotNull().InclusiveBetween(1, config.MaxPageSize).WithName("pageSize");
    }
}

internal class DocumentUpdateValidator : AbstractValidator<DocumentUpdateParameters>
{
    public DocumentUpdateValidator()
    {
        RuleFor(x => x.DisplayName).NotNull();
        RuleFor(x => x.Category).NotEmpty();
        RuleFor(x => x.Company).NotNull();
        RuleFor(x => x.DateOfDocument).NotNull();
        RuleFor(x => x.Description).NotNull();
    }
}

internal class DocumentTagsValidator : AbstractValidator<DocumentTagParameters>
{
    public DocumentTagsValidator()
    {
        RuleFor(x => x.ToAdd).NotNull();
        RuleFor(x => x.ToDelete).NotNull();
    }
}

internal class DocumentMoveValidator : AbstractValidator<DocumentMoveParameters>
{
    public DocumentMoveValidator()
    {
        RuleFor(x => x.FolderFrom).NotEmpty().WithName("folderFrom");
        RuleFor(x => x.FolderTo).NotEmpty().WithName("folderTo");
    }
}