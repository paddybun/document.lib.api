using document.lib.bl.contracts.Categories.Commands;
using document.lib.bl.contracts.Categories.UseCases;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories.UseCases;

public class MergeCategoryUseCase(
    ILogger<MergeCategoryUseCase> logger,
    IMergeCategoryCommand<UnitOfWork> mergeCategoryCommand) : IMergeCategoryUseCase<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, MergeCategoryUseCaseParameters parameters)
    {
        try
        {
            await uow.BeginTransactionAsync();

            var result = await mergeCategoryCommand.ExecuteAsync(uow,
                new MergeCategoryCommandParameters(parameters.SourceCategoryId, parameters.DestinationCategoryId));
            if (!result.IsSuccess)
            {
                await uow.RollbackTransactionAsync();
                return result;
            }

            await uow.CommitAsync();
            logger.LogInformation("Merged category {Source} into {Destination}, {Count} documents moved",
                parameters.SourceCategoryId, parameters.DestinationCategoryId, result.Value);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while merging category {Source} into {Destination}",
                parameters.SourceCategoryId, parameters.DestinationCategoryId);
            await uow.RollbackTransactionAsync();
            return Result<int>.Failure("An error occurred while merging the categories", ex);
        }
    }
}
