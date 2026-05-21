using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Categories.UseCases;

public interface IGetCategoryUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<Category>> ExecuteAsync(T uow, GetCategoryUseCaseParameters parameters);
}

public record GetCategoryUseCaseParameters(string CategoryName);
