namespace document.lib.data.models.RegisterDescriptions;

public class RegisterDescriptionSaveModel
{
    public required string GroupName { get; set; }
    public string? NewGroupName { get; set; }
    public List<RegisterDescriptionEntryModel> Entries { get; set; } = [];
    public bool NeedsMove => !string.IsNullOrWhiteSpace(NewGroupName) && !NewGroupName.Equals(GroupName, StringComparison.OrdinalIgnoreCase);
}