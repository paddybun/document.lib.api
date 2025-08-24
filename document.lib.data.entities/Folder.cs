using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace document.lib.data.entities;

[Index(nameof(Name), IsUnique = true)]
public class Folder: BaseFields
{
    public int Id { get; set; }

    [Required]
    [MaxLength(250)]
    public required string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? DisplayName { get; set; }

    [NotMapped]
    public Register? CurrentRegister => Registers.OrderBy(x => x.Name).FirstOrDefault(x => x.DocumentCount < MaxDocumentsRegister);

    public ICollection<Register> Registers { get; set; } = [];
    
    public int TotalDocuments { get; set; }

    [Required]
    public int MaxDocumentsRegister { get; set; }

    [Required]
    public int MaxDocumentsFolder { get; set; }

    [Required]
    [MaxLength(250)]
    public required string DescriptionGroup { get; set; } = "default";

    public bool IsActive { get; set; } = false;
}