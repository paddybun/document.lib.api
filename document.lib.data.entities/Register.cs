using System.ComponentModel.DataAnnotations;

namespace document.lib.data.entities;

public class Register: BaseFields
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? DisplayName { get; set; }

    public List<Document> Documents { get; set; } = [];

    public Folder? Folder { get; set; }

    [Required]
    public int DocumentCount { get; set; }
}