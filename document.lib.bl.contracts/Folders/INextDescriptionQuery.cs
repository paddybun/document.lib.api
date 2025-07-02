using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Folders;

public interface INextDescriptionQuery
{
    Task<Result<RegisterDescription>> ExecuteAsync(NextDescriptionQueryParameters parameters);
}

public class NextDescriptionQueryParameters
{
    public required int Id { get; set; }
    public required string Group { get; set; }
}