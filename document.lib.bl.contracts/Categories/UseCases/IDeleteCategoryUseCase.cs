using document.lib.core;

namespace document.lib.bl.contracts.Categories.UseCases;

public interface IDeleteCategoryUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, DeleteCategoryUseCaseParameters parameters);
}

public record DeleteCategoryUseCaseParameters(int CategoryId);
