namespace document.lib.shared.Models.QueryDtos;

public class FolderQueryParameters
{
    public FolderQueryParameters(int? id = null, string name = null, bool? activeFolder = null)
    {
        Id = id;
        Name = name;
        ActiveFolder = activeFolder;
    }

    public int? Id { get; set; }
    public string Name { get; set; }
    public bool? ActiveFolder { get; set; }

    public bool IsValid()
    {
        return 
            Id != null || 
            !string.IsNullOrWhiteSpace(Name) || 
            ActiveFolder != null;
    }
}