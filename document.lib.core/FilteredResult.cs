namespace document.lib.core;

public class FilteredResult<T>
{
    public required int Count { get; set; }
    public required IEnumerable<T> FilteredList { get; set; }
}