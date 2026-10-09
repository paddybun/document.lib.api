using document.lib.core;

namespace document.lib.bl.contracts.Categories.Commands;

public interface IMergeCategoryCommand<in T>
    where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, MergeCategoryCommandParameters parameters);
}

public record MergeCategoryCommandParameters(int SourceCategoryId, int DestinationCategoryId);
