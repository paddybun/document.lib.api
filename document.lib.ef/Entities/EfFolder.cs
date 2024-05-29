using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace document.lib.ef.Entities;

public class EfFolder: EfBaseFields
{
    public int Id { get; set; }

    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? DisplayName { get; set; }

    [NotMapped]
    public EfRegister? CurrentRegister => Registers.OrderBy(x => x.Name).FirstOrDefault(x => x.DocumentCount < MaxDocumentsRegister);

    public ICollection<EfRegister> Registers { get; set; } = null!;
    
    public int TotalDocuments { get; set; }

    [Required]
    public int MaxDocumentsRegister { get; set; }

    [Required]
    public int MaxDocumentsFolder { get; set; }

    public bool IsFull { get; set; }
}