namespace document.lib.shared.Models.QueryDtos;

public class DocumentQueryParameters
{

    public DocumentQueryParameters(int? id = null, string name = null)
    {
        Id = id;
        Name = name;
    }

    public int? Id { get; set; }
    public string Name { get; set; }

    public bool IsValid()
    {
        return
            Id != null ||
            !string.IsNullOrWhiteSpace(Name);
    }
}