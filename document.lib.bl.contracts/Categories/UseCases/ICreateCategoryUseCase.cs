using document.lib.core;

namespace document.lib.bl.contracts.Categories.UseCases;

public interface ICreateCategoryUseCase<in T> where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, CreateCategoryUseCaseParameters parameters);
}

public record CreateCategoryUseCaseParameters(string DisplayName);
