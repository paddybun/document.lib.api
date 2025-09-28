using document.lib.core;
using document.lib.data.models.RegisterDescriptions;

namespace document.lib.bl.contracts.RegisterDescriptions;

public interface IRegisterDescriptionSaveUseCase<in T>
    where T : IUnitOfWork
{
    Task<Result<RegisterDescriptionDetailModel>> ExecuteAsync(T uow, RegisterDescriptionSaveUseCaseParameters parameters);
}

public record RegisterDescriptionSaveUseCaseParameters
{
    public required RegisterDescriptionSaveModel SaveModel { get; init; }
}