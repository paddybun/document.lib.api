namespace document.lib.shared.Models.Models;

public class DocumentModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string PhysicalName { get; set; }
    public string BlobLocation { get; set; }
    public string Company { get; set; }
    public DateTimeOffset? DateOfDocument { get; set; }
    public DateTimeOffset UploadDate { get; set; }
    public string Description { get; set; }
    public string RegisterName { get; set; }
    public List<string> Tags { get; set; }
    public bool Unsorted { get; set; }
    public string CategoryId { get; set; }
    public string CategoryName { get; set; }
    public bool Digital { get; set; }
    public string FolderId { get; set; }
    public string FolderName { get; set; }
    public DateTimeOffset DateModified { get; set; }
}