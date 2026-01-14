namespace document.lib.core;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public bool HasError { get; set; }
    public bool HasWarning { get; set; }
    public bool HasData => !(HasError || HasWarning) && Value is not null;
    
    public T? Value { get; private set; }
    public string Message { get; private set; } = "";
    public Exception? Exception { get; set; }

    public static Result<T> Success(T value)
    {
        return new Result<T>
        {
            IsSuccess = true,
            HasError = false,
            Value = value
        };
    }
    
    public static Result<T> Failure(string? errorMessage = "", Exception? exception = null)
    {
        return new Result<T>
        {
            IsSuccess = false,
            HasError = true,
            Value = default
        };
    }
    
    public static Result<T> Warning(string warningMessage)
    {
        return new Result<T>
        {
            IsSuccess = false,
            HasError = false,
            HasWarning = true,
            Message = warningMessage,
            Value = default
        };
    }
}