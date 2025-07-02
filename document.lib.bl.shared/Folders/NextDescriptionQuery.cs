using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.context;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class NextDescriptionQuery(ILogger<NextDescriptionQuery> logger, DatabaseContext context) : INextDescriptionQuery
{
    public async Task<Result<RegisterDescription>> ExecuteAsync(NextDescriptionQueryParameters parameters)
    {
        try
        {
            if (parameters.Id == -1)
            {
                var first = await context.RegisterDescriptions
                    .AsNoTracking()
                    .Where(x => x.Group == parameters.Group)
                    .OrderBy(x => x.Order)
                    .FirstOrDefaultAsync();
                
                if (first == null) {
                    return Result<RegisterDescription>.Warning("No description found in the group");
                }
                
                return Result<RegisterDescription>.Success(first);
            }
            
            var description = await context.RegisterDescriptions
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == parameters.Id && x.Group == parameters.Group);

            if (description == null) return Result<RegisterDescription>.Warning("Description not found");

            // Get the next description in the same group
            var nextDescription = await context.RegisterDescriptions
                .AsNoTracking()
                .Where(x => x.Group == parameters.Group && x.Order > description.Order)
                .OrderBy(x => x.Order)
                .FirstOrDefaultAsync();
            
            if (nextDescription == null) return Result<RegisterDescription>.Warning("No next description found in the group");

            return Result<RegisterDescription>.Success(nextDescription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting next description for parameters {@Parameters}", parameters);
            return Result<RegisterDescription>.Failure();
        }
    }
}