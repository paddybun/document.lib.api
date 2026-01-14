using document.lib.core.Filter;

namespace document.lib.core.Helpers;

public static class FilterHelper
{
    public static FilterDefinition CreateFilter(string[] filterString)
    {
        var filter = new FilterDefinition();
        
        if (filterString.Length <= 0)
            throw new Exception("No filter defined");
        
        foreach (var filterPart in filterString)
        {
            var filterDefinitionParts = filterPart.Split('|');
            if (filterDefinitionParts.Length != 3)
                throw new Exception("Filter definition must have 3 parts separated by '|'");
            if (filterDefinitionParts.Any(string.IsNullOrWhiteSpace))
                throw new Exception("Some filter parts are empty");

            var propertyName = filterDefinitionParts[0];
            var operatorString = filterDefinitionParts[1];
            var value = filterDefinitionParts[2];

            var op = operatorString.ToLower() switch
            {
                "eq" => Operator.Equal,
                "neq" => Operator.NotEqual,
                "gt" => Operator.GreaterThen,
                "gte" => Operator.GreaterThenEqual,
                "lt" => Operator.LessThen,
                "lte" => Operator.LessThenEqual,
                "lk" => Operator.Like,
                "sw" => Operator.StartsWith,
                _ => throw new Exception("Operator is not supported")
            };
            
            var filterDefinition = new FilterPart
            {
                PropertyName = propertyName,
                Operator = op,
                Value = value
            };
            filter.Parts.Add(filterDefinition);
        }

        return filter;
    }
}