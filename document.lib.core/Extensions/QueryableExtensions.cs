using document.lib.core.Helpers;
using document.lib.core.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace document.lib.core.Extensions;

public static class QueryableExtensions
{
    private const int MinRows = 1;
    private const int MaxRows = 500;
    
    public static IQueryable<T> AddSort<T>(this IQueryable<T> me, string[]? sortStrings)
    {
        if (sortStrings is not { Length: > 0 }) return me;
        var def = ExpressionHelper.ToOrderExpression<T>(sortStrings);

        var thenInclude = false;
        foreach (var desc in def.Descending)
        {
            if (thenInclude) me = ((IOrderedQueryable<T>)me).ThenByDescending(desc);
            else
            {
                me = me.OrderByDescending(desc);
                thenInclude = true;
            }
        }

        foreach (var asc in def.Ascending)
        {
            if (thenInclude) me = ((IOrderedQueryable<T>)me).ThenBy(asc);
            else
            {
                me = me.OrderBy(asc);
                thenInclude = true;
            }
        }

        return me;
    }

    public static IQueryable<T> AddWhere<T>(this IQueryable<T> me, string[]? filter)
    {
        if (filter is not { Length: > 0 }) return me;

        var def = FilterHelper.CreateFilter(filter);
        var expr = ExpressionHelper.ToWhereExpression<T>(def);

        return me.Where(expr);
    }

    public static IQueryable<T> AddSelect<T>(this IQueryable<T> me, string[]? fields)
        where T : new()
    {
        if (fields is not { Length: > 0 }) return me;

        var expr = ExpressionHelper.ToSelectExpression<T>(fields);
        return me.Select(expr);
    }

    public static IQueryable<T> AddPagination<T>(this IQueryable<T> me, int skip, int take, bool isCounting)
        where T : new()
    {
        if (isCounting) 
            return me;
        
        var ps = Math.Clamp(take, MinRows, MaxRows);
        return me
            .Skip(skip)
            .Take(ps);
    }

    public static IQueryable<T> AddQueryParameters<T>(this IQueryable<T> me, OverviewRequestParameters parameters, IKey? key, bool isCounting = false)
        where T : new()
    {
        var fieldsToAdd = new List<string>(parameters.Fields ?? []);
        
        // Add default sorting if not provided
        var sort = parameters.Sort;
        if ((sort is null || sort.Length == 0) && key != null)
        {
            var pk = key.Properties.FirstOrDefault()?.Name;
            if (pk is null) 
                throw new InvalidOperationException("To add dynamic query parameters, the entity must have a PK field.");
            
            // Add pk field to selection, in case it is not already there.
            if (fieldsToAdd is { Count: > 0 } && !fieldsToAdd.Contains(pk, StringComparer.InvariantCultureIgnoreCase))
                fieldsToAdd.Add(pk);

            sort = [$"{pk}|asc"];
        }

        var query = me
            .AddSelect(fieldsToAdd.ToArray())
            .AddSort(sort);
            
        if (parameters.Filter is { Length: > 0 })
        {
            query = query.AddWhere(parameters.Filter);
        }
        
        query = query.AddPagination(parameters.Skip, parameters.Take, isCounting);

        return query;
    }
}