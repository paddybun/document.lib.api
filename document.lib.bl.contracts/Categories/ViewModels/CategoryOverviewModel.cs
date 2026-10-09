namespace document.lib.bl.contracts.Categories.ViewModels;

public class CategoryOverviewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public int DocumentCount { get; set; }
}
