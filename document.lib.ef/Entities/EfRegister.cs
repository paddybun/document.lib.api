using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace document.lib.ef.Entities;

[Index(nameof(Name), nameof(Folder), IsUnique = true)]
public class EfRegister: EfBaseFields
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? DisplayName { get; set; }

    public List<EfDocument>? Documents { get; set; }

    public EfFolder? Folder { get; set; }

    [Required]
    public int DocumentCount { get; set; }
}