using System.Security.AccessControl;

namespace document.lib.data.models.RegisterDescriptions;

public class RegisterDescriptionDetailModel
{
    public string Group { get; set; } = null!;
    public List<RegisterDescriptionEntryModel> Entries { get; set; } = [];
}