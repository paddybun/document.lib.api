using document.lib.core;
using document.lib.data.models.RegisterDescriptions;

namespace document.lib.bl.contracts.RegisterDescriptions;

public interface IRegisterDescriptionQuery<in T>
    where T : IUnitOfWork
{
    Task<Result<RegisterDescriptionDetailModel?>> ExecuteAsync(T uow, RegisterDescriptionQueryParameters parameters);
}

public record RegisterDescriptionQueryParameters
{
    public required string GroupName { get; init; }
}