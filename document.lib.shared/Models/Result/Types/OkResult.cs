using document.lib.shared.Enums;

namespace document.lib.shared.Models.Result.Types;

internal class OkResult: IServiceResult
{
    public ServiceResultStatus Status => ServiceResultStatus.Ok;
    public bool IsSuccess => true;
}