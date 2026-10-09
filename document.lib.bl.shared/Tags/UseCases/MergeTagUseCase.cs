using document.lib.bl.contracts.Tags.Commands;
using document.lib.bl.contracts.Tags.UseCases;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Tags.UseCases;

public class MergeTagUseCase(
    ILogger<MergeTagUseCase> logger,
    IMergeTagCommand<UnitOfWork> mergeTagCommand) : IMergeTagUseCase<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, MergeTagUseCaseParameters parameters)
    {
        try
        {
            await uow.BeginTransactionAsync();

            var result = await mergeTagCommand.ExecuteAsync(uow,
                new MergeTagCommandParameters(parameters.SourceTagId, parameters.DestinationTagId));
            if (!result.IsSuccess)
            {
                await uow.RollbackTransactionAsync();
                return result;
            }

            await uow.CommitAsync();
            logger.LogInformation("Merged tag {Source} into {Destination}", parameters.SourceTagId, parameters.DestinationTagId);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while merging tag {Source} into {Destination}", parameters.SourceTagId, parameters.DestinationTagId);
            await uow.RollbackTransactionAsync();
            return Result<int>.Failure("An error occurred while merging the tags", ex);
        }
    }
}
