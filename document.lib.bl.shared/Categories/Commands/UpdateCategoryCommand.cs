using document.lib.bl.contracts.Categories.Commands;
using document.lib.core;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Categories.Commands;

public class UpdateCategoryCommand : IUpdateCategoryCommand<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, UpdateCategoryCommandParameters parameters)
    {
        var displayName = parameters.DisplayName?.Trim();
        if (string.IsNullOrEmpty(displayName) || displayName.Length > 500)
            return Result<int>.Warning("Display name is required and must not exceed 500 characters.");

        var category = await uow.Connection.Categories.SingleOrDefaultAsync(x => x.Id == parameters.CategoryId);
        if (category == null)
            return Result<int>.Warning($"Category {parameters.CategoryId} not found.");

        if (category.Name == Constants.UncategorizedName)
            return Result<int>.Warning("The system category cannot be edited.");

        category.DisplayName = displayName;
        return Result<int>.Success(category.Id);
    }
}
