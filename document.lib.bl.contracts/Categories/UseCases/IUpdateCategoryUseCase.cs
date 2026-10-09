using document.lib.core;

namespace document.lib.bl.contracts.Categories.UseCases;

public interface IUpdateCategoryUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, UpdateCategoryUseCaseParameters parameters);
}

public record UpdateCategoryUseCaseParameters(int CategoryId, string DisplayName);
