using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace document.lib.data.entities;

public class Folder: BaseFields
{
    public int Id { get; set; }

    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? DisplayName { get; set; }

    [NotMapped]
    public Register? CurrentRegister => Registers.OrderBy(x => x.Name).FirstOrDefault(x => x.DocumentCount < MaxDocumentsRegister);

    public ICollection<Register> Registers { get; set; } = null!;
    
    public int TotalDocuments { get; set; }

    [Required]
    public int MaxDocumentsRegister { get; set; }

    [Required]
    public int MaxDocumentsFolder { get; set; }

    public bool IsDefault { get; set; } = false;

    public bool IsFull { get; set; }
}