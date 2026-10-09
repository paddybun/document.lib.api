using document.lib.bl.contracts.Documents.Queries;
using document.lib.bl.contracts.Documents.UseCases;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Documents.UseCases;

public class GetCompaniesUseCase(ILogger<GetCompaniesUseCase> logger, ICompaniesQuery<UnitOfWork> query)
    : IGetCompaniesUseCase<UnitOfWork>
{
    public async Task<Result<List<string>>> ExecuteAsync(UnitOfWork uow)
    {
        try
        {
            var result = await query.ExecuteAsync(uow);
            return result.HasError ? Result<List<string>>.Failure(result.Message) : result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting companies");
            return Result<List<string>>.Failure(ex.Message, ex);
        }
    }
}
