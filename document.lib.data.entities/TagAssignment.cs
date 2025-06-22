using System.ComponentModel.DataAnnotations;

namespace document.lib.data.entities;

public class TagAssignment: BaseFields
{
    public int Id { get; set; }

    [Required]
    public Document Document { get; set; } = null!;

    [Required]
    public Tag Tag { get; set; } = null!;
}