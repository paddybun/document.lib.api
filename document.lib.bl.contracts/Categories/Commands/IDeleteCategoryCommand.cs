using document.lib.core;

namespace document.lib.bl.contracts.Categories.Commands;

public interface IDeleteCategoryCommand<in T>
    where T : IUnitOfWork
{
    Task<Result<int>> ExecuteAsync(T uow, DeleteCategoryCommandParameters parameters);
}

public record DeleteCategoryCommandParameters(int CategoryId);
