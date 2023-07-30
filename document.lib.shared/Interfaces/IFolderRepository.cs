using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Interfaces;

public interface IFolderRepository
{
    Task<FolderModel> GetFolderAsync(FolderQueryParameters queryParameters);
    Task<List<FolderModel>> GetAllFoldersAsync();
    Task<FolderModel> CreateFolderAsync(FolderModel folder);
    Task<FolderModel> UpdateFolderAsync(FolderModel folder);
    Task AddDocumentToFolderAsync(FolderModel folder, DocumentModel document);
    Task RemoveDocFromFolderAsync(FolderModel folder, DocumentModel document);
}