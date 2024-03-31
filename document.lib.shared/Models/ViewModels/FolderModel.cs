using document.lib.ef.Entities;

namespace document.lib.shared.Models.ViewModels;

public class FolderModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public RegisterModel CurrentRegister { get; set; } = null!;
    public string CurrentRegisterName { get; set; } = null!;
    public List<RegisterModel> Registers { get; set; } = [];
    public int TotalDocuments { get; set; }
    public int DocumentsRegister { get; set; }
    public int DocumentsFolder { get; set; }
    public bool IsFull { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}