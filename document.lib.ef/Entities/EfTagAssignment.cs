using System.ComponentModel.DataAnnotations;

namespace document.lib.ef.Entities;

public class EfTagAssignment: EfBaseFields
{
    public int Id { get; set; }

    [Required]
    public EfDocument Document { get; set; } = null!;

    [Required]
    public EfTag Tag { get; set; } = null!;
}