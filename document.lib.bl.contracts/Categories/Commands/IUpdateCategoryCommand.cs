using document.lib.core;

namespace document.lib.bl.contracts.Categories.Commands;

public interface IUpdateCategoryCommand<in T>
    where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, UpdateCategoryCommandParameters parameters);
}

public record UpdateCategoryCommandParameters(int CategoryId, string DisplayName);
