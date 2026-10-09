namespace document.lib.data.models.Documents;

public sealed class DocumentSaveModel
{
    public required string DisplayName { get; set; }
    public required DateTimeOffset DateOfDocument { get; set; }
    public string? Company { get; set; }
    public string? Description { get; set; }
    public required int CategoryId { get; set; }
    public bool Digital { get; set; } = false;
    public List<string> Tags { get; set; } = [];
}
