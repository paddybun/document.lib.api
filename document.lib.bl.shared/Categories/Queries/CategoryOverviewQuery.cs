using document.lib.bl.contracts.Categories.Queries;
using document.lib.bl.contracts.Categories.ViewModels;
using document.lib.core;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Categories.Queries;

public class CategoryOverviewQuery : ICategoryOverviewQuery<UnitOfWork>
{
    public async Task<Result<List<CategoryOverviewModel>>> ExecuteAsync(UnitOfWork uow, CategoryOverviewQueryParameters parameters)
    {
        var query = uow.Connection.Categories.AsNoTracking()
            .Where(x => x.Name != Constants.UncategorizedName);

        if (parameters.CategoryId is { } id)
            query = query.Where(x => x.Id == id);

        var categories = await query
            .OrderBy(x => x.DisplayName ?? x.Name)
            .Select(x => new CategoryOverviewModel
            {
                Id = x.Id,
                Name = x.Name,
                DisplayName = x.DisplayName,
                DocumentCount = uow.Connection.Documents.Count(d => d.CategoryId == x.Id)
            })
            .ToListAsync();

        return Result<List<CategoryOverviewModel>>.Success(categories);
    }
}
