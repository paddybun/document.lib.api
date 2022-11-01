using document.lib.shared.TableEntities;

namespace document.lib.shared.Models;

public class MetadataResponse
{
    public DocLibTag[] Tags { get; set; }
    public DocLibCategory[] Categories { get; set; }
    public DocLibFolder[] Folders { get; set; }
}