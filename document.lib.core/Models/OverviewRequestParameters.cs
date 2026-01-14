namespace document.lib.core.Models;

public class OverviewRequestParameters
{
    public string[]? Fields { get; set; }
    
    public string[]? Filter { get; set; }
    
    public string[]? Sort { get; set; }

    public required int Skip { get; set; }

    public required int Take { get; set; }
}