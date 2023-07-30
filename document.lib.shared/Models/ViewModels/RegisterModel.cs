using document.lib.ef.Entities;

namespace document.lib.shared.Models.ViewModels;

public class RegisterModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public List<DocumentModel> Documents { get; set; }
    public string FolderId { get; set; }
    public string FolderName { get; set; }
    public int DocumentCount { get; set; }
}