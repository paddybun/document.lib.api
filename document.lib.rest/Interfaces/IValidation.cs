using System.Linq.Expressions;

namespace document.lib.rest.Interfaces;

public class PropertyValidator
{
    public static bool ValidateHasValue<T>(T objToValidate, params Expression<Func<T, int?>>[] expressions)
    {
        List<bool> results = new();
        foreach (var expression in expressions)
        {
            if (expression.Body is MemberExpression)
            {
                var propValue = expression.Compile()(objToValidate);
                results.Add(propValue.HasValue);
            }
        }
        return results.All(x => x);
    }
    
    public static bool ValidateHasValue<T>(T objToValidate, params Expression<Func<T, string?>>[] expressions)
    {
        List<bool> results = new();
        foreach (var expression in expressions)
        {
            if (expression.Body is MemberExpression)
            {
                var propValue = expression.Compile()(objToValidate);
                results.Add(!string.IsNullOrWhiteSpace(propValue));
            }
        }
        return results.All(x => x);
    }
}