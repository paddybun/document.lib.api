using document.lib.shared.Models.Data;

namespace document.lib.shared.Interfaces;

public interface IFolderService
{
    Task<FolderModel?> GetFolderByNameAsync(string name);
    Task<FolderModel?> GetFolderByIdAsync(int id);
    Task<FolderModel> CreateNewFolderAsync(int docsPerFolder = 150, int docsPerRegister = 10, string? displayName = null);
    Task<FolderModel> GetOrCreateActiveFolderAsync();
    Task <List<FolderModel>> GetAllAsync();
    Task<(int, List<FolderModel>)> GetFoldersPaged(int page, int pageSize);
    Task<FolderModel?> UpdateFolderAsync(FolderModel folder);
    Task AddDocumentToFolderAsync(FolderModel folder, DocumentModel doc);
    Task RemoveDocumentFromFolder(FolderModel folder, DocumentModel doc);
}