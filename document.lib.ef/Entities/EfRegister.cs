namespace document.lib.ef.Entities;

public class EfRegister: EfBaseFields
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public List<EfDocument> Documents { get; set; } = [];
    public EfFolder Folder { get; set; }
    public int DocumentCount { get; set; }
}