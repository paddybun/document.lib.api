using document.lib.data.entities;
using Mapster;

namespace document.lib.data.models.Documents;

public sealed class DocumentSaveModel
{
    public string? DisplayName { get; set; }
    public string? Company { get; set; }
    public DateTimeOffset? DateOfDocument { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public List<string> Tags { get; set; } = [];

    private DocumentSaveModel() { }
    
    public static DocumentSaveModel Create(Document document)
    {
        var model = new DocumentSaveModel();
        document.Adapt(model);

        var tags = document.Tags
            .Select(x => x.Tag.Name)
            .ToList();
        
        model.Tags = tags;
        
        return model;
    }
    
    public Document ToEntity(Document document)
    {
        this.Adapt(document);
        return document;
    }
}