namespace document.lib.data.models.Folders;

public class FolderView
{
    public int FolderId { get; set; }
    public List<FolderViewItem> Items { get; set; } = [];
}

public class FolderViewItem
{
    public int RegisterId { get; set; }
    public int DocumentId { get; set; }
    public string Register { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
}