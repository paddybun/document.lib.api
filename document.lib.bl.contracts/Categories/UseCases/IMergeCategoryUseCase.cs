using document.lib.core;

namespace document.lib.bl.contracts.Categories.UseCases;

public interface IMergeCategoryUseCase<in T>
    where T : IUnitOfWork
{
    /// <returns>The number of moved documents.</returns>
    Task<Result<int>> ExecuteAsync(T uow, MergeCategoryUseCaseParameters parameters);
}

public record MergeCategoryUseCaseParameters(int SourceCategoryId, int DestinationCategoryId);
