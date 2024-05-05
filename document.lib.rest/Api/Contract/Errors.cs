namespace document.lib.rest.Api.Contract;

public record ValidationError(string PropertyName, string Message);