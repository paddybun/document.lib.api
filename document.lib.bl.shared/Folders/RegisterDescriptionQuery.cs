using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.models.RegisterDescriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class RegisterDescriptionQuery(ILogger<RegisterDescriptionQuery> logger): IRegisterDescriptionQuery<UnitOfWork>
{
    public async Task<Result<RegisterDescriptionDetailModel?>> ExecuteAsync(UnitOfWork uow, RegisterDescriptionQueryParameters parameters)
    {
        try
        {
            logger.LogDebug("Executing {name} with parameters {@Parameters}", nameof(RegisterDescriptionQuery), parameters);
            logger.LogInformation("Executing {name}", nameof(RegisterDescriptionQuery));
            
            var descriptions = await uow.Connection.RegisterDescriptions
                .AsNoTracking()
                .Where(rd => rd.Group == parameters.GroupName)
                .OrderBy(rd => rd.Order)
                .ToListAsync();

            if (!descriptions.Any())
            {
                return Result<RegisterDescriptionDetailModel?>.Warning("No register descriptions found for the specified group.");
            }

            var entries = descriptions
                .Select(x => new RegisterDescriptionEntryModel
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName, 
                    Order = x.Order
                })
                .ToList();
            var model = new RegisterDescriptionDetailModel
            {
                Group = parameters.GroupName,
                Entries = entries.ToList()
            };
            
            return Result<RegisterDescriptionDetailModel?>.Success(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving register descriptions with parameters {@Parameters}", parameters);
            return Result<RegisterDescriptionDetailModel?>.Failure("An error occurred while retrieving register descriptions.");
        }
    }
}