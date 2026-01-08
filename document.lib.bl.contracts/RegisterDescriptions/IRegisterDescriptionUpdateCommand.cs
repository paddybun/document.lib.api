using document.lib.core;
using document.lib.data.models.RegisterDescriptions;

namespace document.lib.bl.contracts.RegisterDescriptions;

public interface IRegisterDescriptionUpdateCommand<in T>
    where T : IUnitOfWork
{
    Task<Result<bool>> ExecuteAsync(T uow, RegisterDescriptionUpdateCommandParameters parameters);
}

public record RegisterDescriptionUpdateCommandParameters
{
    public required RegisterDescriptionSaveModel SaveModel { get; init; }
}