using System.ComponentModel.DataAnnotations;

namespace document.lib.data.entities;

public class Document: BaseFields
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
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string BlobLocation { get; set; } = null!;

    [MaxLength(250)]
    public string? Company { get; set; }
    public DateTimeOffset? DateOfDocument { get; set; }
    public DateTimeOffset UploadDate { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }
    
    public int RegisterId { get; set; }
    public Register Register { get; set; } = null!;
    
    public List<TagAssignment> Tags { get; set; } = null!;
    public bool Unsorted { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    public bool Digital { get; set; }
}