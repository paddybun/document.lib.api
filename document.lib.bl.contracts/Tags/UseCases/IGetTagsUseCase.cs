using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Tags.UseCases;

public interface IGetTagsUseCase<in T> where T : IUnitOfWork
{
    Task<Result<IEnumerable<Tag>>> ExecuteAsync(T uow);
}
