using System.Linq.Expressions;

namespace document.lib.core.Sort;

public class SortDefinition<T>
{
    public List<Expression<Func<T, object>>> Ascending { get; set; } = [];
    public List<Expression<Func<T, object>>> Descending { get; set; } = [];
}