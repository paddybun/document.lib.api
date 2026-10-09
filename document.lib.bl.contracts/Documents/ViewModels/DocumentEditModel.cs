namespace document.lib.bl.contracts.Documents.ViewModels;

public class DocumentEditModel
{
    public required int Id { get; set; }
    public string? DisplayName { get; set; }
    public DateTimeOffset? DateOfDocument { get; set; }
    public int? CategoryId { get; set; }
    public List<int> TagIds { get; set; } = [];

    public string Filename { get; set; } = string.Empty;
    public bool Unsorted { get; set; }
    public string Folder { get; set; } = string.Empty;
    public string Register { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset UploadDate { get; set; }
    public DateTimeOffset DateModified { get; set; }
    public bool Digital { get; set; }
    public int FilesizeInBytes { get; set; }
}
