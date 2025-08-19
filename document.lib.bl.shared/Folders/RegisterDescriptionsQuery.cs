using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.core.System;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class RegisterDescriptionsQuery(
    ILogger<RegisterDescriptionsQuery> logger): IRegisterDescriptionsQuery<UnitOfWork>
{
    public async Task<Result<List<RegisterDescription>>> ExecuteAsync(UnitOfWork uow, RegisterDescriptionsQueryParameters parameters)
    {
        try
        {
            logger.LogInformation("RegisterDescriptionsQuery started");

            var query = uow.Connection.RegisterDescriptions
                .AsNoTracking();
            
            if (parameters.HideSystemDescriptions)
            {
                query = query.Where(rd => 
                    rd.Group != SystemConstants.UnsortedRegisterName &&
                    rd.Group != SystemConstants.DigitalRegisterName);
            }
            
            var descriptions = await query
                .ToListAsync();
            
            
            return Result<List<RegisterDescription>>.Success(descriptions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving register descriptions");
            return Result<List<RegisterDescription>>.Failure();
        }
    }
}