using document.lib.bl.contracts.Categories.Commands;
using document.lib.bl.contracts.Categories.UseCases;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories.UseCases;

public class UpdateCategoryUseCase(
    ILogger<UpdateCategoryUseCase> logger,
    IUpdateCategoryCommand<UnitOfWork> updateCategoryCommand) : IUpdateCategoryUseCase<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, UpdateCategoryUseCaseParameters parameters)
    {
        try
        {
            await uow.BeginTransactionAsync();

            var result = await updateCategoryCommand.ExecuteAsync(uow,
                new UpdateCategoryCommandParameters(parameters.CategoryId, parameters.DisplayName));
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
            logger.LogError(ex, "Error occurred while updating category {CategoryId}", parameters.CategoryId);
            await uow.RollbackTransactionAsync();
            return Result<int>.Failure("An error occurred while updating the category", ex);
        }
    }
}
