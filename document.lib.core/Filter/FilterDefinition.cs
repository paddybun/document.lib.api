namespace document.lib.core.Filter;

public class FilterDefinition
{
    public Predicate Predicate { get; set; } = Predicate.And;
    public List<FilterPart> Parts { get; set; } = [];
}