using document.lib.ef.Entities;

namespace document.lib.shared.Models.ViewModels;

public class FolderModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public RegisterModel CurrentRegister { get; set; }
    public string CurrentRegisterName { get; set; }
    public List<RegisterModel> Registers { get; set; }
    public int TotalDocuments { get; set; }
    public int DocumentsRegister { get; set; }
    public int DocumentsFolder { get; set; }
    public bool IsFull { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}