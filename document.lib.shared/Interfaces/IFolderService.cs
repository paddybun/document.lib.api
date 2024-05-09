using document.lib.shared.Enums;
using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;

namespace document.lib.shared.Interfaces;

public interface IFolderService
{
    Task<ITypedServiceResult<FolderModel?>> GetFolderAsync(int id);
    Task<ITypedServiceResult<FolderModel?>> GetFolderAsync(string name);
    Task<ITypedServiceResult<FolderModel?>> GetActiveFolderAsync(RetrievalOptions options = RetrievalOptions.GetOnly);
    Task<ITypedServiceResult<FolderModel>> CreateNewFolderAsync(int docsPerFolder = 150, int docsPerRegister = 10, string? displayName = null);
    Task<ITypedServiceResult<List<FolderModel>>> GetAllAsync();
    Task<ITypedServiceResult<(int, List<FolderModel>)>> GetFoldersPaged(int page, int pageSize);
    Task<ITypedServiceResult<FolderModel?>> UpdateFolderAsync(FolderModel folder);
}