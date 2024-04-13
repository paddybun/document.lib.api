using System.ComponentModel.DataAnnotations;

namespace document.lib.ef.Entities;

public class EfDocument: EfBaseFields
{
    public int Id { get; set; }

    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? DisplayName { get; set; }

    [Required]
    [MaxLength(500)]
    public string PhysicalName { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string BlobLocation { get; set; } = null!;

    [MaxLength(250)]
    public string? Company { get; set; }
    public DateTimeOffset? DateOfDocument { get; set; }
    public DateTimeOffset UploadDate { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }
    public EfRegister Register { get; set; } = null!;
    public List<EfTagAssignment>? Tags { get; set; }
    public bool Unsorted { get; set; }
    public EfCategory Category { get; set; } = null!;
    public bool Digital { get; set; }
}