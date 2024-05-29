using FluentValidation;

namespace document.lib.rest.Api;

public static class ValidationHelper {
    public static IResult? Validate<T>(IValidator<T> validator, T obj)
    {
        var validationResult = validator.Validate(obj);
        return !validationResult.IsValid ? Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToArray()) : null;
    }
}