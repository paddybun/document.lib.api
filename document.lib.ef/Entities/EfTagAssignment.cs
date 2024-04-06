namespace document.lib.ef.Entities;

public class EfTagAssignment: EfBaseFields
{
    public int Id { get; set; }
    public EfDocument Document { get; set; }
    public EfTag EfTag { get; set; }
}