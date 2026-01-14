using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Tags.Queries;

public interface ITagsQuery<in T> 
    where T: IUnitOfWork
{
    Task<Result<IEnumerable<Tag>>> ExecuteAsync(T uow);
}