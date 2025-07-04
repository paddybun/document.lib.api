using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class RegisterDescriptionsQuery(
    ILogger<RegisterDescriptionsQuery> logger,
    DatabaseContext context): IRegisterDescriptionsQuery
{
    public async Task<Result<List<RegisterDescription>>> ExecuteAsync()
    {
        try
        {
            logger.LogInformation("RegisterDescriptionsQuery started");
            
            var descriptions = await context.RegisterDescriptions
                .AsNoTracking()
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