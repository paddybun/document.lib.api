using System.Linq.Expressions;
using document.lib.core.Filter;
using document.lib.core.Sort;

namespace document.lib.core.Helpers;

public static class ExpressionHelper
{
    public static Expression<Func<T, T>> ToSelectExpression<T>(string[] fields)
        where T : new()
    {
        if (fields is not { Length: > 0 }) throw new Exception("No fields defined");
        
        var newObjectExpression = Expression.New(typeof(T));
        var param = Expression.Parameter(typeof(T), "x");

        var assignments = new List<MemberAssignment>();

        foreach (var fld in fields)
        {
            var propertyToSet = typeof(T).GetProperty(fld);
            
            if (propertyToSet == null) 
                throw new Exception($"Property '{fld}' does not exist on type '{typeof(T).Name}'");
            
            var prop = Expression.Property(param, typeof(T), fld);
            var assignment = Expression.Bind(propertyToSet, prop);
            assignments.Add(assignment);
        }

        var memberInit = Expression.MemberInit(newObjectExpression, assignments);
        var lambda = Expression.Lambda<Func<T, T>>(memberInit, param);
        return lambda;
    }
    
    public static Expression<Func<T, bool>> ToWhereExpression<T>(FilterDefinition definition)
    {
        var entityType = typeof(T);
        
        var param = Expression.Parameter(typeof(T), "x");
        if (definition.Parts.Count <= 0) throw new Exception("No filter parts found");

        List<Expression> bodyExpressions = [];
        foreach (var part in definition.Parts)
        {
            var property = entityType.GetProperty(part.PropertyName);
            if (property == null)
            {
                throw new Exception($"Property '{part.PropertyName}' does not exist on type '{entityType.Name}'");
            }
            
            var propertyExpression = Expression.Property(param, part.PropertyName);
            var t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var value = Convert.ChangeType(part.Value, t);
            
            var constant = Expression.Constant(value);
            var conv = Expression.Convert(constant, property.PropertyType);

            Expression tmpbody = part.Operator switch
            {
                Operator.Equal => Expression.Equal(propertyExpression, conv),
                Operator.NotEqual => Expression.NotEqual(propertyExpression, conv),
                Operator.GreaterThen => Expression.GreaterThan(propertyExpression, conv),
                Operator.GreaterThenEqual => Expression.GreaterThanOrEqual(propertyExpression, conv),
                Operator.LessThen => Expression.LessThan(propertyExpression, conv),
                Operator.LessThenEqual => Expression.LessThanOrEqual(propertyExpression, conv),
                Operator.Like => Expression.Call(propertyExpression,
                    typeof(string).GetMethod("Contains", [typeof(string)])!, conv),
                Operator.StartsWith => Expression.Call(propertyExpression,
                    typeof(string).GetMethod("StartsWith", [typeof(string)])!, conv),
                _ => null!
            };
            bodyExpressions.Add(tmpbody);
        }
        
        var body = definition.Predicate switch
        {
            Predicate.And => bodyExpressions.Aggregate(Expression.AndAlso),
            Predicate.Or => bodyExpressions.Aggregate(Expression.OrElse),
            _ => throw new Exception("Predicate not supported")
        };
        
        var lambda = Expression.Lambda<Func<T, bool>>(body, param);
        return lambda;
    }
    
    public static SortDefinition<T> ToOrderExpression<T>(string[] sortFields)
    {
        if (sortFields is not { Length: > 0 }) throw new Exception("No sort fields defined");
        
        var def = new SortDefinition<T>();
        var sortDefinitions = sortFields
            .Select(x => x.Split("|"))
            .Select(x => new { Name=x[0], Order=x[1] });

        var entityType = typeof(T);
        var param = Expression.Parameter(typeof(T), "x");
        
        foreach (var sort in sortDefinitions)
        {
            var property = entityType.GetProperty(sort.Name);
            if (property == null)
            {
                throw new Exception($"Property '{sort.Name}' does not exist on type '{entityType.Name}'");
            }
            
            var propertyExpression = Expression.Property(param, sort.Name);
            var converted = Expression.Convert(propertyExpression, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(converted, param);

            switch (sort.Order.ToLower())
            {
                case "asc":
                    def.Ascending.Add(lambda);
                    break;
                case "desc":
                    def.Descending.Add(lambda);
                    break;
                default:
                    throw new Exception("Sort order not supported");
            }
        }
        
        return def;
    }
}