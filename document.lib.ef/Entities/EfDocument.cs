namespace document.lib.ef.Entities;

public class EfDocument: BaseFields
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string PhysicalName { get; set; }
    public string BlobLocation { get; set; }
    public string Company { get; set; }
    public DateTimeOffset? DateOfDocument { get; set; }
    public DateTimeOffset UploadDate { get; set; }
    public string Description { get; set; }
    public EfRegister Register { get; set; }
    public ICollection<EfTag> Tags { get; set; }
    public bool Unsorted { get; set; }
    public EfCategory Category { get; set; }
    public bool Digital { get; set; }
}