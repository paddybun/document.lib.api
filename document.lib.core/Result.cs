namespace document.lib.core;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; private set; }
    public bool HasWarning { get; set; }
    public string Message { get; private set; } = "";
    public Exception? Exception { get; set; }

    public static Result<T> Success(T value)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Value = value
        };
    }
    
    public static Result<T> Failure(string? errorMessage = "", Exception? exception = null)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Value = default
        };
    }
    
    public static Result<T> Warning(string warningMessage)
    {
        return new Result<T>
        {
            IsSuccess = false,
            HasWarning = true,
            Message = warningMessage,
            Value = default
        };
    }
}