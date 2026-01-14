namespace document.lib.bl.contracts.Documents.ViewModels;

public class DocumentOverviewModel
{
    public required int Id { get; set; }
    public bool Unsorted { get; set; } = false;
    public required string DisplayName { get; set; }
    public required string Filename { get; set; }
    public required string Folder { get; set; }
    public required string Register { get; set; }
}