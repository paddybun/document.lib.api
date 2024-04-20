using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface IFolderService
{
    Task<FolderModel?> GetFolderByNameAsync(string name);
    Task<FolderModel?> GetFolderByIdAsync(string id);
    Task<FolderModel?> GetFolderByIdAsync(int id);
    Task<FolderModel> CreateNewFolderAsync(FolderModel model);
    Task<FolderModel> GetOrCreateActiveFolderAsync();
    Task<FolderModel> SaveAsync(FolderModel folderModel, bool createNew = false);
    Task<FolderModel> GetOrCreateFolderByIdAsync(string name);
    Task <List<FolderModel>> GetAllAsync();
    Task<FolderModel?> UpdateFolderAsync(FolderModel folder);
    Task AddDocumentToFolderAsync(FolderModel folder, DocumentModel doc);
    Task RemoveDocumentFromFolder(FolderModel folder, DocumentModel doc);
    Task<(int, FolderModel[])> GetFoldersPaged(int page, int pageSize);
}