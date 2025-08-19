using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface IRegisterDescriptionsQuery<in T>
    where T : IUnitOfWork
{
    Task<Result<List<RegisterDescription>>> ExecuteAsync(T uow, RegisterDescriptionsQueryParameters parameters);
}

public record RegisterDescriptionsQueryParameters
{
    public bool HideSystemDescriptions { get; set; } = true;
}