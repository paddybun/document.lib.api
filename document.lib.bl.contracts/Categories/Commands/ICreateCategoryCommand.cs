using document.lib.core;
using document.lib.data.entities;

namespace document.lib.bl.contracts.Categories.Commands;

public interface ICreateCategoryCommand<in T> where T : IUnitOfWork
{
    Task<Result<Category>> ExecuteAsync(T uow, CreateCategoryCommandParameters parameters);
}

public record CreateCategoryCommandParameters(string DisplayName);
