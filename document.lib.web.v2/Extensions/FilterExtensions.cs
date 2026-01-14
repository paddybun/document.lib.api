using Radzen;

namespace document.lib.web.v2.Extensions;

public static class FilterExtensions
{
    public static string ToFilterString(this FilterDescriptor descriptor)
    {
        var op = descriptor.FilterOperator switch
        {
            FilterOperator.Equals => "eq",
            FilterOperator.Contains => "sw",
            FilterOperator.NotEquals => "neq",
            FilterOperator.LessThan => "lt",
            FilterOperator.LessThanOrEquals => "lte",
            FilterOperator.GreaterThan => "gt",
            FilterOperator.GreaterThanOrEquals => "gte",
            FilterOperator.StartsWith => "sw",
            FilterOperator.EndsWith => "ew",
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{descriptor.Property}|{op}|{descriptor.FilterValue}";
    }
}