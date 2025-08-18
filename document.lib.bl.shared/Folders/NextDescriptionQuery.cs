using document.lib.bl.contracts.Folders;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Folders;

public class NextDescriptionQuery(ILogger<NextDescriptionQuery> logger) : INextDescriptionQuery<UnitOfWork>
{
    public async Task<Result<RegisterDescription>> ExecuteAsync(UnitOfWork uow, NextDescriptionQueryParameters parameters)
    {
        try
        {
            if (parameters.IsNew)
            {
                var first = await uow.Connection.RegisterDescriptions
                    .AsNoTracking()
                    .Where(x => x.Group == parameters.Group)
                    .OrderBy(x => x.Order)
                    .FirstOrDefaultAsync();
                
                if (first == null) {
                    return Result<RegisterDescription>.Warning("No description found in the group");
                }
                
                return Result<RegisterDescription>.Success(first);
            }

            if (parameters.Id <= 0) return Result<RegisterDescription>.Failure(errorMessage: "Please supply a valid id");
            
            var description = await uow.Connection.RegisterDescriptions
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == parameters.Id && x.Group == parameters.Group);

            if (description == null) return Result<RegisterDescription>.Warning("Description not found");

            // Get the next description in the same group
            var nextDescription = await uow.Connection.RegisterDescriptions
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