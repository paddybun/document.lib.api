using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Interfaces;

public interface IFolderService
{
    Task<FolderModel> GetFolderByNameAsync(string name);
    Task<FolderModel> GetOrCreateActiveFolderAsync();
    Task<FolderModel> CreateNewFolderAsync(int maxFolderSize = 200, int maxRegisterSize = 10);
    Task<FolderModel> GetOrCreateFolderByIdAsync(string name, int maxFolderSize = 200, int maxRegisterSize = 10);
    Task <List<FolderModel>> GetAllAsync();
    Task<FolderModel> UpdateFolderAsync(FolderModel folder);
    Task AddDocumentToFolderAsync(FolderModel folder, DocumentModel doc);
    Task RemoveDocumentFromFolder(FolderModel folder, DocumentModel doc);
}