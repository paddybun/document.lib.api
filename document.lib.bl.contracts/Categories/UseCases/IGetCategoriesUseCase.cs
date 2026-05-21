using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Categories.UseCases;

public interface IGetCategoriesUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<List<Category>>> ExecuteAsync(T uow, GetCategoriesUseCaseParameters parameters);
}

public record GetCategoriesUseCaseParameters(int? Skip, int? Take);
