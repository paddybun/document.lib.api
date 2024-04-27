using System.Linq.Expressions;

namespace document.lib.shared.Helper;

public static class PropertyChecker
{
    public static class Values
    {
        /// <summary>
        /// Evaluates if all the given expressions are true. Avoid using this method for heavy workloads. 
        /// </summary>
        /// <param name="objToValidate">The object to perform property checks on</param>
        /// <param name="expressions">The expressions to check</param>
        /// <typeparam name="TObj">The object type</typeparam>
        /// <returns>True if all checks succeed</returns>
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
                        checkedValues.Add(false);
                        break;
                }
            }
            return checkedValues.All(x => x);
        }
        
        /// <summary>
        /// Evaluates if any of the given expressions are true. Avoid using this method for heavy workloads. 
        /// </summary>
        /// <param name="objToValidate">The object to perform property checks on</param>
        /// <param name="expressions">The expressions to check</param>
        /// <typeparam name="TObj">The object type</typeparam>
        /// <returns>True if any checks succeed</returns>
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
                        hasValue = false;
                        break;
                }

                if (hasValue) break;
            }
            return hasValue;
        }
    }
}