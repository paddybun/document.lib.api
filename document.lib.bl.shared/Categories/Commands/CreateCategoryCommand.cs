using document.lib.bl.contracts.Categories.Commands;
using document.lib.core;
using document.lib.data.entities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Categories.Commands;

public class CreateCategoryCommand : ICreateCategoryCommand<UnitOfWork>
{
    public async Task<Result<Category>> ExecuteAsync(UnitOfWork uow, CreateCategoryCommandParameters parameters)
    {
        var displayName = parameters.DisplayName?.Trim();
        if (string.IsNullOrEmpty(displayName) || displayName.Length > 500)
            return Result<Category>.Warning("Display name is required and must not exceed 500 characters.");

        var normalizedName = displayName.ToLowerInvariant();
        if (normalizedName == Constants.UncategorizedName)
            return Result<Category>.Warning("The name 'uncategorized' is reserved for the system category.");

        if (await uow.Connection.Categories.AnyAsync(x =>
                (x.DisplayName ?? x.Name).ToLower() == normalizedName))
            return Result<Category>.Warning("A category with this display name already exists.");

        var category = new Category
        {
            Name = Guid.NewGuid().ToString(),
            DisplayName = displayName
        };
        await uow.Connection.Categories.AddAsync(category);
        return Result<Category>.Success(category);
    }
}
