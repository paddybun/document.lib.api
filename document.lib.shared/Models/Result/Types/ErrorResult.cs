using document.lib.shared.Enums;

namespace document.lib.shared.Models.Result.Types;

internal class ErrorResult: IServiceResult
{
    public ServiceResultStatus Status => ServiceResultStatus.Error;
    public bool IsSuccess => false;
}