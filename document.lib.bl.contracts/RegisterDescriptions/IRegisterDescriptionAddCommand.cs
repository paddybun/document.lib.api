using document.lib.core;
using document.lib.data.models.RegisterDescriptions;

namespace document.lib.bl.contracts.RegisterDescriptions;

public interface IRegisterDescriptionAddCommand<in T>
    where T : IUnitOfWork
{
    Task<Result<bool>> ExecuteAsync(T uow, RegisterDescriptionAddCommandParameters parameters);
}

public record RegisterDescriptionAddCommandParameters
{
    public required RegisterDescriptionSaveModel SaveModel { get; init; }
}