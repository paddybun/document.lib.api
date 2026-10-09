using document.lib.bl.contracts.Documents.Queries;
using document.lib.core;
using Microsoft.EntityFrameworkCore;

namespace document.lib.bl.shared.Documents.Queries;

public class CompaniesQuery : ICompaniesQuery<UnitOfWork>
{
    public async Task<Result<List<string>>> ExecuteAsync(UnitOfWork uow)
    {
        var companies = await uow.Connection.Documents
            .AsNoTracking()
            .Where(x => x.Company != null && x.Company != "")
            .Select(x => x.Company!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        return Result<List<string>>.Success(companies);
    }
}
