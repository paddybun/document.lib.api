using document.lib.core;

namespace document.lib.bl.contracts.RegisterDescriptions;

public interface IRegisterDescriptionRenameGroupCommand<in T>
    where T : IUnitOfWork
{
    Task<Result<bool>> ExecuteAsync(T uow, RegisterDescriptionMoveToNewGroupCommandParameters parameters);
}

public record RegisterDescriptionMoveToNewGroupCommandParameters
{
    public required string OldGroupName { get; init; }
    public required string NewGroupName { get; init; }
}