using document.lib.bl.contracts.Categories.Commands;
using document.lib.core;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Categories.Commands;

public class DeleteCategoryCommand : IDeleteCategoryCommand<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, DeleteCategoryCommandParameters parameters)
    {
        var category = await uow.Connection.Categories.SingleOrDefaultAsync(x => x.Id == parameters.CategoryId);
        if (category == null)
            return Result<int>.Warning($"Category {parameters.CategoryId} not found.");

        if (category.Name == Constants.UncategorizedName)
            return Result<int>.Warning("The system category cannot be deleted.");

        if (await uow.Connection.Documents.AnyAsync(x => x.CategoryId == category.Id))
            return Result<int>.Warning("The category is still used by documents.");

        uow.Connection.Categories.Remove(category);
        return Result<int>.Success(category.Id);
    }
}
