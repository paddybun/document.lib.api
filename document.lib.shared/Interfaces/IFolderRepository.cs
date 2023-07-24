using document.lib.shared.Models.QueryDtos;
using document.lib.shared.TableEntities;
using DocLibDocument = document.lib.shared.TableEntities.DocLibDocument;

namespace document.lib.shared.Interfaces;

public interface IFolderRepository
{
    Task<DocLibFolder> GetFolderAsync(FolderQueryParameters queryParameters);
    Task<List<DocLibFolder>> GetAllFoldersAsync();
    Task<DocLibFolder> CreateFolderAsync(DocLibFolder folder);
    Task UpdateFolderAsync(DocLibFolder folder);
    Task AddDocumentToFolderAsync(DocLibFolder folder, DocLibDocument document);
    Task RemoveDocFromFolderAsync(DocLibFolder folder, DocLibDocument document);
}