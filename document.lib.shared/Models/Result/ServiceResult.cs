using document.lib.shared.Models.Result.Types;

namespace document.lib.shared.Models.Result;

public static class ServiceResult
{
    public static IServiceResult Ok() => new OkResult();
    public static IServiceResult Error() => new ErrorResult();

    public static ITypedServiceResult<TMe> Ok<TMe>(TMe? data) => new OkDataResult<TMe>(data);
    public static ITypedServiceResult<TMe> Error<TMe>(TMe? data) => new ErrorDataResult<TMe>(data);
}