namespace document.lib.ef.Entities;

public class Folder: BaseFields
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public Register CurrentRegister { get; set; }
    public ICollection<Register> Registers { get; set; }
    public int TotalDocuments { get; set; }
    public int MaxDocumentsRegister { get; set; }
    public int MaxDocumentsFolder { get; set; }
    public bool IsFull { get; set; }
}