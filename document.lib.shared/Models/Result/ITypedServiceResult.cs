namespace document.lib.shared.Models.Result;

public interface ITypedServiceResult<out T> : IServiceResult
{
    public T? Data { get; }
}