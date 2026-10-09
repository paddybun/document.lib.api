namespace document.lib.data.models.Documents;

public sealed class DocumentSaveModel
{
    public required string DisplayName { get; set; }
    public required DateTimeOffset DateOfDocument { get; set; }
    public required int CategoryId { get; set; }
    public List<string> Tags { get; set; } = [];
}
