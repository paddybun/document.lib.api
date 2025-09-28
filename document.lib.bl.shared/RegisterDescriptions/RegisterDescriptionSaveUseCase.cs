using document.lib.bl.contracts.RegisterDescriptions;
using document.lib.core;
using document.lib.data.models.RegisterDescriptions;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.RegisterDescriptions;

public class RegisterDescriptionSaveUseCase(
    ILogger<RegisterDescriptionSaveUseCase> logger,
    IRegisterDescriptionQuery<UnitOfWork> descriptionQuery,
    IRegisterDescriptionAddCommand<UnitOfWork> addCommand,
    IRegisterDescriptionRenameGroupCommand<UnitOfWork> renameCommand,
    IRegisterDescriptionUpdateCommand<UnitOfWork> updateCommand): IRegisterDescriptionSaveUseCase<UnitOfWork>
{
    public async Task<Result<RegisterDescriptionDetailModel>> ExecuteAsync(UnitOfWork uow, RegisterDescriptionSaveUseCaseParameters parameters)
    {
        try
        {
            await uow.BeginTransactionAsync();
            logger.LogDebug("Executing {useCase} with parameters {@Parameters}", nameof(RegisterDescriptionSaveUseCase), parameters);
            logger.LogInformation("Executing {useCase}", nameof(RegisterDescriptionSaveUseCase));

            var existing = await descriptionQuery.ExecuteAsync(uow, new() { GroupName = parameters.SaveModel.GroupName });
            var isNew = existing is not { IsSuccess: true, Value: not null };

            if (isNew)
            {
                var addResult = await addCommand.ExecuteAsync(uow, new() { SaveModel = parameters.SaveModel });
                if (addResult is not { IsSuccess: true, Value: true })
                {
                    await uow.RollbackTransactionAsync();
                    return Result<RegisterDescriptionDetailModel>.Failure("Failed to add new register descriptions.");
                }
            }
            else
            {
                if (parameters.SaveModel.NeedsMove)
                {
                    if (parameters.SaveModel.GroupName.Equals(parameters.SaveModel.NewGroupName,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        await uow.RollbackTransactionAsync();
                        return Result<RegisterDescriptionDetailModel>.Failure("New group name must be different from the current group name.");
                    }
                    
                    var renameResult = await renameCommand.ExecuteAsync(uow, new()
                    {
                        OldGroupName = parameters.SaveModel.GroupName, NewGroupName = parameters.SaveModel.NewGroupName!
                    });

                    if (renameResult is not { IsSuccess: true, Value: true })
                    {
                        await uow.RollbackTransactionAsync();
                        return Result<RegisterDescriptionDetailModel>.Failure("Failed to move register descriptions to the new group.");
                    }
                }
                
                var updateResult = await updateCommand.ExecuteAsync(uow, new() { SaveModel = parameters.SaveModel });
                if (updateResult is not { IsSuccess: true, Value: true })
                {
                    await uow.RollbackTransactionAsync();
                    return Result<RegisterDescriptionDetailModel>.Failure("Failed to update register descriptions.");
                }
            }
            
            await uow.CommitAsync();
            var queryResult = await descriptionQuery.ExecuteAsync(uow, new()
            {
                GroupName = parameters.SaveModel.NeedsMove ? parameters.SaveModel.NewGroupName! : parameters.SaveModel.GroupName
            });
            
            if (queryResult is not { IsSuccess: true, Value: not null })
            {
                return Result<RegisterDescriptionDetailModel>.Failure("Failed to retrieve updated register descriptions.");
            }
            
            return Result<RegisterDescriptionDetailModel>.Success(queryResult.Value);
        }
        catch (Exception ex)
        {
            logger.LogError("Error executing {useCase} with parameters {@Parameters}: {ErrorMessage}", nameof(RegisterDescriptionSaveUseCase), parameters, ex.Message);
            return Result<RegisterDescriptionDetailModel>.Failure("An error occurred while saving register descriptions.");
        }
    }
}