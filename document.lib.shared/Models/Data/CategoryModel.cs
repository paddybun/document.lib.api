namespace document.lib.shared.Models.Data;

public class CategoryModel
{
    public object? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset DateModified { get; set; }
}