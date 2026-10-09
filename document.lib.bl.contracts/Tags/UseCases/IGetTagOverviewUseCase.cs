using document.lib.bl.contracts.Tags.ViewModels;
using document.lib.core;

namespace document.lib.bl.contracts.Tags.UseCases;

public interface IGetTagOverviewUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<List<TagOverviewModel>>> ExecuteAsync(T uow);
}
