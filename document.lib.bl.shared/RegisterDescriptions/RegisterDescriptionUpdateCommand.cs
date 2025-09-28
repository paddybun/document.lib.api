using document.lib.bl.contracts.RegisterDescriptions;
using document.lib.core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.RegisterDescriptions;

public class RegisterDescriptionUpdateCommand(ILogger<RegisterDescriptionUpdateCommand> logger): IRegisterDescriptionUpdateCommand<UnitOfWork>
{
    public async Task<Result<bool>> ExecuteAsync(UnitOfWork uow, RegisterDescriptionUpdateCommandParameters parameters)
    {
        try
        {
            logger.LogDebug("Executing {Command} with parameters: {Parameters}", nameof(RegisterDescriptionUpdateCommand), parameters);
            logger.LogInformation("Executing {Command}", nameof(RegisterDescriptionUpdateCommand));

            foreach (var entry in parameters.SaveModel.Entries)
            {
                await uow.Connection.RegisterDescriptions
                    .Where(x => x.Id == entry.Id)
                    .ExecuteUpdateAsync(setter => setter
                        .SetProperty(x => x.DisplayName, entry.DisplayName)
                        .SetProperty(x => x.Order, entry.Order));
            }
            
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error executing {Command} with parameters: {Parameters}", nameof(RegisterDescriptionUpdateCommand), parameters);
            return Result<bool>.Failure("An error occurred while updating the register description.");
        }
    }
}