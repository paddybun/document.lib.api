namespace document.lib.core.Filter;

public class FilterPart
{
    public string PropertyName { get; set; } = null!;
    public Operator Operator { get; set; }
    public string Value { get; set; } = null!;
}