using document.lib.ef.Entities;
using document.lib.shared.Models.Data;

namespace document.lib.shared.Models.Update;

public record FolderUpdateModel (
    int Id,
    bool IsFull,
    int TotalDocuments,
    int DocsPerRegister = 10,
    int DocsPerFolder = 150,
    string? DisplayName = null)
{
    public static FolderUpdateModel CreateFromEntity(EfFolder folder)
    {
        return new FolderUpdateModel(folder.Id, folder.IsFull, folder.TotalDocuments, folder.MaxDocumentsRegister,
            folder.MaxDocumentsFolder, folder.DisplayName);
    }
    
    public static FolderUpdateModel CreateFromModel(FolderModel folder)
    {
        return new FolderUpdateModel((int)folder.Id!, folder.IsFull, folder.TotalDocuments, folder.DocumentsRegister,
            folder.DocumentsFolder, folder.DisplayName);
    }
}