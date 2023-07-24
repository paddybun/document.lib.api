namespace document.lib.ef.Entities;

public class EfFolder: BaseFields
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public EfRegister CurrentRegister { get; set; }
    public ICollection<EfRegister> Registers { get; set; }
    public int TotalDocuments { get; set; }
    public int MaxDocumentsRegister { get; set; }
    public int MaxDocumentsFolder { get; set; }
    public bool IsFull { get; set; }
}