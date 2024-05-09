using document.lib.shared.Enums;

namespace document.lib.shared.Models.Result.Types;

internal class OkDataResult<TIn>(TIn? data) : ITypedServiceResult<TIn>
{
    public ServiceResultStatus Status => ServiceResultStatus.Ok;
    public bool IsSuccess => true;
    public TIn? Data { get; } = data;
}