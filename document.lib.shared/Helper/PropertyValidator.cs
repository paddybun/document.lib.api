using System.Linq.Expressions;

namespace document.lib.shared.Helper;

public static class PropertyValidator
{
    public static bool AllHaveValue<TObj>(TObj objToValidate, params Expression<Func<TObj, object?>>[] expressions)
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
                case { } o:
                    checkedValues.Add(true);
                    break;
                default:
                    checkedValues.Add(false);
                    break;
            }
        }
        return checkedValues.All(x => x);
    }
    
    public static bool AnyHasValue<TObj> (TObj objToValidate, params Expression<Func<TObj, object?>>[] expressions)
    {
        bool hasValue = false;
        foreach (var expression in expressions)
        {
            if (expression.Body is not (UnaryExpression or MemberExpression)) continue;
            var compiledValue = expression.Compile()(objToValidate);
            
            switch (compiledValue)
            {
                case string s:
                    hasValue = !string.IsNullOrWhiteSpace(s);
                    break;
                case { } o:
                    hasValue = true;
                    break;
                default:
                    hasValue = false;
                    break;
            }

            if (hasValue) break;
        }
        return hasValue;
    }
}