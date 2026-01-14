using System.Text;

namespace document.lib.core.Helpers;

public class GenericStringBuilder(string defaultSeparator) : IGenericStringBuilder
{
    private bool _hasPreviousValue = false;
    private readonly StringBuilder _sb = new();

    public IGenericStringBuilder WithValue(string? value = null, string? defaultSeparatorOverride = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return this;
        }
        defaultSeparatorOverride ??= defaultSeparator;

        if (_hasPreviousValue)
        {
            _sb.Append(defaultSeparatorOverride);
            _sb.Append(value.Trim());
        }
        else
        {
            _sb.Append(value.Trim());
        }

        _hasPreviousValue = true;
        return this;
    }

    public IGenericStringBuilder WithValues(IEnumerable<string?> values, string? separator = null, string? defaultSeparatorOverride = null)
    {
        var v = values.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        if (v.Length == 0)
        {
            return this;
        }
        
        var partSeparator = separator ?? defaultSeparator;
        var firstSeparator = defaultSeparatorOverride ?? defaultSeparator;

        var isFirstValue = true;
        foreach (var value in v)
        {
            if (_hasPreviousValue)
            {
                _sb.Append(isFirstValue ? firstSeparator : partSeparator);
                _sb.Append(value!.Trim());
            }
            else
            {
                _sb.Append(value!.Trim());
            }

            isFirstValue = false;
            _hasPreviousValue = true;
        }
        
        return this;
    }

    public string Build()
    {
        return ToString();
    }

    public override string ToString()
    {
        return _sb.ToString();
    }
}

public interface IGenericStringBuilder 
{
    IGenericStringBuilder WithValue(string? separator = null, string? defaultSeparatorOverride = null);
    IGenericStringBuilder WithValues(IEnumerable<string?> values, string? separator = null, string? defaultSeparatorOverride = null);
    string Build();
}