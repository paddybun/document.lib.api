using document.lib.shared.Enums;

namespace document.lib.shared.Models.Result;

public interface IServiceResult
{
    public ServiceResultStatus Status { get; }
    public bool IsSuccess { get; }
}