using document.lib.bl.contracts.Categories.Commands;
using document.lib.bl.contracts.Categories.UseCases;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories.UseCases;

public class DeleteCategoryUseCase(
    ILogger<DeleteCategoryUseCase> logger,
    IDeleteCategoryCommand<UnitOfWork> deleteCategoryCommand) : IDeleteCategoryUseCase<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, DeleteCategoryUseCaseParameters parameters)
    {
        try
        {
            await uow.BeginTransactionAsync();

            var result = await deleteCategoryCommand.ExecuteAsync(uow, new DeleteCategoryCommandParameters(parameters.CategoryId));
            if (!result.IsSuccess)
            {
                await uow.RollbackTransactionAsync();
                return result;
            }

            await uow.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting category {CategoryId}", parameters.CategoryId);
            await uow.RollbackTransactionAsync();
            return Result<int>.Failure("An error occurred while deleting the category", ex);
        }
    }
}
