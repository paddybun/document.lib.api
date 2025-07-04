using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface IRegisterDescriptionsQuery
{
    Task<Result<List<RegisterDescription>>> ExecuteAsync();
}