namespace document.lib.bl.contracts.Tags.ViewModels;

public class TagOverviewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int DocumentCount { get; set; }
}
