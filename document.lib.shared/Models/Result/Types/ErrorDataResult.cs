using document.lib.shared.Enums;

namespace document.lib.shared.Models.Result.Types;

internal class ErrorDataResult<TIn>(TIn? data): ITypedServiceResult<TIn>
{
    public ServiceResultStatus Status => ServiceResultStatus.Error;
    public bool IsSuccess => false;
    public TIn? Data { get; } = data;
}