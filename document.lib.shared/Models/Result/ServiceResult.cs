using document.lib.shared.Models.Result.Types;

namespace document.lib.shared.Models.Result;

public static class ServiceResult
{
    public static IServiceResult Ok() => new OkResult();
    public static IServiceResult Error() => new ErrorResult();

    public static ITypedServiceResult<T> Ok<T>(T? data) => new OkDataResult<T>(data);
    public static ITypedServiceResult<T> Error<T>(T? data, string? message = null) => new ErrorDataResult<T>(data, message);
    public static ITypedServiceResult<T> DefaultError<T>(string? message = null) => new ErrorDataResult<T>(default, message);
}