using document.lib.bl.contracts.RegisterDescriptions;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.RegisterDescriptions;

public class RegisterDescriptionAddCommand(ILogger<RegisterDescriptionAddCommand> logger): IRegisterDescriptionAddCommand<UnitOfWork>
{
    public async Task<Result<bool>> ExecuteAsync(UnitOfWork uow, RegisterDescriptionAddCommandParameters parameters)
    {
        try
        {
            logger.LogDebug("Executing {useCase} with parameters {@Parameters}", nameof(RegisterDescriptionAddCommand), parameters);
            logger.LogInformation("Executing {useCase}", nameof(RegisterDescriptionAddCommand));

            foreach (var entry in parameters.SaveModel.Entries)
            {
                var registerDescriptionEntity = new RegisterDescription
                {
                    Name = Guid.NewGuid().ToString(),
                    DisplayName = entry.DisplayName,
                    Order = entry.Order,
                    Group = parameters.SaveModel.GroupName
                };
                
                await uow.Connection.RegisterDescriptions.AddAsync(registerDescriptionEntity);
            }
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing {useCase}", nameof(RegisterDescriptionAddCommand));
            return Result<bool>.Failure("An error occurred while adding a register description.");
        }
    }
}