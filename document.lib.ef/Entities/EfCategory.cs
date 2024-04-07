using System.ComponentModel.DataAnnotations;

namespace document.lib.ef.Entities;

public class EfCategory: EfBaseFields
{
    public int Id { get; set; }

    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? DisplayName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}