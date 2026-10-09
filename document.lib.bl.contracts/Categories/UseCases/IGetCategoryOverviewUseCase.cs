using document.lib.bl.contracts.Categories.ViewModels;
using document.lib.core;

namespace document.lib.bl.contracts.Categories.UseCases;

public interface IGetCategoryOverviewUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<List<CategoryOverviewModel>>> ExecuteAsync(T uow, GetCategoryOverviewUseCaseParameters parameters);
}

public record GetCategoryOverviewUseCaseParameters(int? CategoryId = null);
