using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace document.lib.data.entities;

[Index(nameof(Group), nameof(Order), IsUnique = true, AllDescending = true)]
public class RegisterDescription : BaseFields
{
    public int Id { get; set; }

    [Required]
    [MaxLength(250)]
    public required string Name { get; set; } = null!;

    [Required]
    [MaxLength(250)]
    public required string DisplayName { get; set; } = null!;

    [Required]
    [MaxLength(250)]
    public string Group { get; set; } = "default";
    
    public int Order { get; set; }
}