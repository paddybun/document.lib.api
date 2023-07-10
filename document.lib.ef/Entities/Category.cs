namespace document.lib.ef.Entities;

public class Category: BaseFields
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
}