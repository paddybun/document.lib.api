using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface INextDescriptionQuery<in T>
    where T : IUnitOfWork
{
    Task<Result<RegisterDescription>> ExecuteAsync(T uow, NextDescriptionQueryParameters parameters);
}

public class NextDescriptionQueryParameters
{
    public required int Id { get; set; }
    public required bool IsNew { get; set; }
    public required string Group { get; set; }
}