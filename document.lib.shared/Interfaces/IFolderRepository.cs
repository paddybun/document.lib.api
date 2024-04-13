using document.lib.shared.Models.Models;
using document.lib.shared.Models.QueryDtos;

namespace document.lib.shared.Interfaces;

public interface IFolderRepository
{
    Task<FolderModel?> GetFolderAsync(FolderModel folderModel);
    Task<List<FolderModel>> GetAllFoldersAsync();
    Task<FolderModel> CreateFolderAsync(FolderModel folder);
    Task<FolderModel> UpdateFolderAsync(FolderModel folder);
    Task AddDocumentToFolderAsync(FolderModel folder, DocumentModel document);
    Task RemoveDocFromFolderAsync(FolderModel folder, DocumentModel document);
}