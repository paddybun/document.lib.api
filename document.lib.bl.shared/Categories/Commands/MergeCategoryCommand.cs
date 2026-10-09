using document.lib.bl.contracts.Categories.Commands;
using document.lib.core;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Categories.Commands;

public class MergeCategoryCommand : IMergeCategoryCommand<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, MergeCategoryCommandParameters parameters)
    {
        if (parameters.SourceCategoryId == parameters.DestinationCategoryId)
            return Result<int>.Warning("Source and destination category must differ.");

        var ids = new[] { parameters.SourceCategoryId, parameters.DestinationCategoryId };
        var categories = await uow.Connection.Categories.Where(x => ids.Contains(x.Id)).ToListAsync();
        var source = categories.SingleOrDefault(x => x.Id == parameters.SourceCategoryId);
        var destination = categories.SingleOrDefault(x => x.Id == parameters.DestinationCategoryId);

        if (source == null || destination == null)
            return Result<int>.Warning("Source or destination category not found.");

        if (source.Name == Constants.UncategorizedName || destination.Name == Constants.UncategorizedName)
            return Result<int>.Warning("The system category cannot be part of a merge.");

        var documents = await uow.Connection.Documents
            .Where(x => x.CategoryId == source.Id)
            .ToListAsync();

        foreach (var document in documents)
            document.CategoryId = destination.Id;

        return Result<int>.Success(documents.Count);
    }
}
