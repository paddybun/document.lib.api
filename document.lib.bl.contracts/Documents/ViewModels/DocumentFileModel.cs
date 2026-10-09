namespace document.lib.bl.contracts.Documents.ViewModels;

public class DocumentFileModel
{
    public required Stream Content { get; set; }
    public required string FileName { get; set; }
}
