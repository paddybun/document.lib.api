using document.lib.bl.contracts.Categories.Commands;
using document.lib.bl.contracts.Categories.UseCases;
using document.lib.core;
using Microsoft.Extensions.Logging;

namespace document.lib.bl.shared.Categories.UseCases;

public class CreateCategoryUseCase(
    ILogger<CreateCategoryUseCase> logger,
    ICreateCategoryCommand<UnitOfWork> createCategoryCommand) : ICreateCategoryUseCase<UnitOfWork>
{
    public async Task<Result<int>> ExecuteAsync(UnitOfWork uow, CreateCategoryUseCaseParameters parameters)
    {
        try
        {
            await uow.BeginTransactionAsync();
            var result = await createCategoryCommand.ExecuteAsync(uow,
                new CreateCategoryCommandParameters(parameters.DisplayName));
            if (!result.HasData)
            {
                await uow.RollbackTransactionAsync();
                return result.HasWarning
                    ? Result<int>.Warning(result.Message)
                    : Result<int>.Failure(result.Message);
            }

            await uow.CommitAsync();
            return Result<int>.Success(result.Value!.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating category");
            await uow.RollbackTransactionAsync();
            return Result<int>.Failure("An error occurred while creating the category", ex);
        }
    }
}
