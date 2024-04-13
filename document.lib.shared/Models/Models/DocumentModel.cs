namespace document.lib.shared.Models.Models;

public class DocumentModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string PhysicalName { get; set; } = null!;
    public string BlobLocation { get; set; } = null!;
    public string? Company { get; set; }
    public DateTimeOffset? DateOfDocument { get; set; }
    public DateTimeOffset UploadDate { get; set; }
    public string? Description { get; set; }
    public string RegisterName { get; set; } = null!;
    public List<string>? Tags { get; set; }
    public bool Unsorted { get; set; }
    public string CategoryId { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public bool Digital { get; set; }
    public string? FolderId { get; set; }
    public string? FolderName { get; set; }
    public DateTimeOffset DateModified { get; set; }
}