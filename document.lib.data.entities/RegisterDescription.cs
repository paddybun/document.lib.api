using System.ComponentModel.DataAnnotations;

namespace document.lib.data.entities;

public class RegisterDescription : BaseFields
{
    public int Id { get; set; }

    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(250)]
    public string DisplayName { get; set; } = null!;

    [Required]
    [MaxLength(250)]
    public string Group { get; set; } = "default";
    
    public int Order { get; set; }
}