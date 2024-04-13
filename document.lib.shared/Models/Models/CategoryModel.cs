namespace document.lib.shared.Models.Models;

public class CategoryModel
{
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset DateModified { get; set; }
}