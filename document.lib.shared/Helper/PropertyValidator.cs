using System.Linq.Expressions;

namespace document.lib.shared.Helper;

public static class PropertyValidator
{
    public static bool All<TObj>(TObj objToValidate, params Expression<Func<TObj, object?>>[] expressions)
    {
        List<bool> checkedValues = [];
        foreach (var expression in expressions)
        {
            if (expression.Body is not (UnaryExpression or MemberExpression)) continue;
            var compiledValue = expression.Compile()(objToValidate);
            
            switch (compiledValue)
            {
                case string s:
                    checkedValues.Add(!string.IsNullOrWhiteSpace(s));
                    break;
                case not null:
                    checkedValues.Add(true);
                    break;
                case null:
                default:
                    checkedValues.Add(false);
                    break;
            }
        }
        return checkedValues.All(x => x);
    }
    
    public static bool Any<TObj> (TObj objToValidate, params Expression<Func<TObj, object?>>[] expressions)
    {
        var hasValue = false;
        foreach (var expression in expressions)
        {
            if (expression.Body is not (UnaryExpression or MemberExpression)) continue;
            var compiledValue = expression.Compile()(objToValidate);
            
            switch (compiledValue)
            {
                case string s:
                    hasValue = !string.IsNullOrWhiteSpace(s);
                    break;
                case not null:
                    hasValue = true;
                    break;
                case null:
                default:
                    hasValue = false;
                    break;
            }

            if (hasValue) break;
        }
        return hasValue;
    }
}