using document.lib.data.entities;
using Mapster;

namespace document.lib.data.models.Folders;

public class FolderSaveModel(bool createNew)
{
    // Required
    public bool CreateNew { get; set; } = createNew;
    public int MaxDocumentsRegister { get; set; }
    public int MaxDocumentsFolder { get; set; }
    public required string DescriptionGroup { get; set; } = null!;
    
    // Optional
    public int? Id { get; set; }
    public string? DisplayName { get; set; }
    
    public Folder ApplyToEntity(Folder folder)
    {
        this.Adapt(folder);
        return folder;
    }
}