namespace document.lib.ef.Entities;

public class Register: BaseFields
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public ICollection<DocLibDocument> Documents { get; set; }
    public Folder Folder { get; set; }
    public int DocumentCount { get; set; }
}