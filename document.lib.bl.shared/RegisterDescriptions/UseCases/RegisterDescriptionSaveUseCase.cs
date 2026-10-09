using document.lib.bl.contracts.RegisterDescriptions.Commands;
using document.lib.bl.contracts.RegisterDescriptions.Queries;
using document.lib.bl.contracts.RegisterDescriptions.UseCases;
using document.lib.core;
using document.lib.core.System;
using document.lib.data.models.RegisterDescriptions;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.RegisterDescriptions.UseCases;

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
            var validation = Validate(parameters.SaveModel);
            if (validation != null)
                return Result<RegisterDescriptionDetailModel>.Warning(validation);

            await uow.BeginTransactionAsync();
            logger.LogDebug("Executing {useCase} with parameters {@Parameters}", nameof(RegisterDescriptionSaveUseCase), parameters);
            logger.LogInformation("Executing {useCase}", nameof(RegisterDescriptionSaveUseCase));

            var existing = await descriptionQuery.ExecuteAsync(uow, new() { GroupName = parameters.SaveModel.GroupName });
            var isNew = existing is not { IsSuccess: true, Value: not null };

            if (parameters.SaveModel.CreateNew && !isNew)
            {
                await uow.RollbackTransactionAsync();
                return Result<RegisterDescriptionDetailModel>.Warning("A register set with this name already exists.");
            }

            if (!isNew && parameters.SaveModel.NeedsMove)
            {
                var target = await descriptionQuery.ExecuteAsync(uow, new() { GroupName = parameters.SaveModel.EffectiveGroupName });
                if (target is { IsSuccess: true, Value: not null })
                {
                    await uow.RollbackTransactionAsync();
                    return Result<RegisterDescriptionDetailModel>.Warning("A register set with this name already exists.");
                }
            }

            var inUse = !isNew && existing.Value!.InUse;
            if (inUse && EntriesChanged(existing.Value!, parameters.SaveModel))
            {
                await uow.RollbackTransactionAsync();
                return Result<RegisterDescriptionDetailModel>.Warning("This register set is in use by a folder. Its entries cannot be changed; copy it as a new set instead.");
            }

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
                
                var updateResult = inUse
                    ? Result<bool>.Success(true)
                    : await updateCommand.ExecuteAsync(uow, new() { SaveModel = parameters.SaveModel });
                if (updateResult is not { IsSuccess: true, Value: true })
                {
                    await uow.RollbackTransactionAsync();
                    if (updateResult.HasWarning)
                        return Result<RegisterDescriptionDetailModel>.Warning(updateResult.Message);

                    return Result<RegisterDescriptionDetailModel>.Failure("Failed to update register descriptions.");
                }
            }
            
            await uow.CommitAsync();
            var queryResult = await descriptionQuery.ExecuteAsync(uow, new()
            {
                GroupName = parameters.SaveModel.EffectiveGroupName
            });
            
            if (queryResult is not { IsSuccess: true, Value: not null })
            {
                return Result<RegisterDescriptionDetailModel>.Failure("Failed to retrieve updated register descriptions.");
            }
            
            return Result<RegisterDescriptionDetailModel>.Success(queryResult.Value);
        }
        catch (Exception ex)
        {
            await uow.RollbackTransactionAsync();
            logger.LogError("Error executing {useCase} with parameters {@Parameters}: {ErrorMessage}", nameof(RegisterDescriptionSaveUseCase), parameters, ex.Message);
            return Result<RegisterDescriptionDetailModel>.Failure("An error occurred while saving register descriptions.");
        }
    }

    private static bool EntriesChanged(RegisterDescriptionDetailModel stored, RegisterDescriptionSaveModel submitted)
    {
        var current = stored.Entries.OrderBy(x => x.Order).Select(x => (x.Id, x.DisplayName));
        var requested = submitted.Entries.OrderBy(x => x.Order).Select(x => (x.Id, x.DisplayName));
        return !current.SequenceEqual(requested);
    }

    private static string? Validate(RegisterDescriptionSaveModel model)
    {
        var group = model.EffectiveGroupName?.Trim();
        if (string.IsNullOrWhiteSpace(group) || group.Length > 250)
            return "The register set needs a name with at most 250 characters.";

        if (IsSystemGroup(group) || IsSystemGroup(model.GroupName))
            return "System register sets cannot be managed.";

        if (model.Entries.Count == 0)
            return "A register set needs at least one entry.";

        foreach (var entry in model.Entries)
        {
            entry.DisplayName = entry.DisplayName?.Trim() ?? string.Empty;
            if (entry.DisplayName.Length is 0 or > 250)
                return "Every entry needs a display name with at most 250 characters.";
        }

        if (model.Entries.Select(x => x.DisplayName).Distinct(StringComparer.OrdinalIgnoreCase).Count() != model.Entries.Count)
            return "Entry display names must be unique.";

        // contiguous order following the submitted sequence
        var ordered = model.Entries.OrderBy(x => x.Order).ToList();
        for (var i = 0; i < ordered.Count; i++) ordered[i].Order = i;

        if (model.NeedsMove) model.NewGroupName = model.NewGroupName!.Trim();
        return null;
    }

    private static bool IsSystemGroup(string? group) =>
        string.Equals(group, SystemConstants.UnsortedRegisterName, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(group, SystemConstants.DigitalRegisterName, StringComparison.OrdinalIgnoreCase);
}
