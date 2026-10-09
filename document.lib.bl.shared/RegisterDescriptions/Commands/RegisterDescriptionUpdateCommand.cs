using document.lib.bl.contracts.RegisterDescriptions.Commands;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.RegisterDescriptions.Commands;

public class RegisterDescriptionUpdateCommand(ILogger<RegisterDescriptionUpdateCommand> logger): IRegisterDescriptionUpdateCommand<UnitOfWork>
{
    public async Task<Result<bool>> ExecuteAsync(UnitOfWork uow, RegisterDescriptionUpdateCommandParameters parameters)
    {
        logger.LogDebug("Executing {Command} with parameters: {@Parameters}", nameof(RegisterDescriptionUpdateCommand), parameters);

        var model = parameters.SaveModel;
        var group = model.EffectiveGroupName;

        // (Group, Order) is unique, so park all rows on unique negative orders before assigning the final ones
        await uow.Connection.RegisterDescriptions
            .Where(x => x.Group == group)
            .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.Order, x => -x.Id));

        var existing = await uow.Connection.RegisterDescriptions.Where(x => x.Group == group).ToListAsync();
        var keptIds = model.Entries.Where(x => x.Id > 0).Select(x => x.Id).ToHashSet();

        if (keptIds.Any(id => existing.All(x => x.Id != id)))
            return Result<bool>.Warning("An entry does not belong to this register set.");

        var removed = existing.Where(x => !keptIds.Contains(x.Id)).ToList();
        if (removed.Count > 0)
        {
            var removedIds = removed.Select(x => x.Id).ToList();
            if (await uow.Connection.Registers.AnyAsync(x => removedIds.Contains(x.DescriptionId)))
                return Result<bool>.Warning("Entries that are already used by registers cannot be removed.");

            uow.Connection.RegisterDescriptions.RemoveRange(removed);
        }

        foreach (var entry in model.Entries)
        {
            if (entry.Id > 0)
            {
                var entity = existing.Single(x => x.Id == entry.Id);
                entity.DisplayName = entry.DisplayName;
                entity.Order = entry.Order;
            }
            else
            {
                await uow.Connection.RegisterDescriptions.AddAsync(new RegisterDescription
                {
                    Name = Guid.NewGuid().ToString(),
                    DisplayName = entry.DisplayName,
                    Order = entry.Order,
                    Group = group
                });
            }
        }

        return Result<bool>.Success(true);
    }
}
