using document.lib.bl.contracts.RegisterDescriptions;
using document.lib.core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.RegisterDescriptions;

public class RegisterDescriptionRenameGroupCommand(ILogger<RegisterDescriptionRenameGroupCommand> logger): IRegisterDescriptionRenameGroupCommand<UnitOfWork>
{
    public async Task<Result<bool>> ExecuteAsync(UnitOfWork uow, RegisterDescriptionMoveToNewGroupCommandParameters parameters)
    {
        try
        {
            logger.LogDebug("Executing {command} with parameters {@Parameters}", nameof(RegisterDescriptionRenameGroupCommand), parameters);
            logger.LogInformation("Executing {command}", nameof(RegisterDescriptionRenameGroupCommand));

            await uow.Connection.Folders
                .Where(x => x.DescriptionGroup.Equals(parameters.OldGroupName, StringComparison.OrdinalIgnoreCase))
                .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.DescriptionGroup, parameters.NewGroupName));
            
            await uow.Connection.RegisterDescriptions
                .Where(x => x.Group.Equals(parameters.OldGroupName, StringComparison.OrdinalIgnoreCase))
                .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.Group, parameters.NewGroupName));
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError("Error executing {command} with parameters {@Parameters}: {ErrorMessage}", nameof(RegisterDescriptionRenameGroupCommand), parameters, ex.Message);
            return Result<bool>.Failure("An error occurred while moving register descriptions to a new group.");
        }
    }
}