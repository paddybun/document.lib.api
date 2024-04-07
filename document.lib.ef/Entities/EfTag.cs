using System.ComponentModel.DataAnnotations;

namespace document.lib.ef.Entities;

public class EfTag: EfBaseFields
{
    public int Id { get; set; }

    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? DisplayName { get; set; }
}