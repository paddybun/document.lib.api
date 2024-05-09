using document.lib.shared.Models.Result.Types;

namespace document.lib.shared.Models.Result;

public static class ServiceResult
{
    public static IServiceResult Ok() => new OkResult();
    public static IServiceResult Error() => new ErrorResult();

    public static ITypedServiceResult<T> Ok<T>(T? data) => new OkDataResult<T>(data);
    public static ITypedServiceResult<T> Error<T>(T? data) => new ErrorDataResult<T>(data);
    public static ITypedServiceResult<T> ErrorDefault<T>() => new ErrorDataResult<T>(default);
}