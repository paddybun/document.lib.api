using document.lib.data.entities;
using document.lib.shared.Models.Data;

namespace document.lib.shared.Models.Update;

public record DocumentUpdateModel (
    string? Category = null,
    string? DisplayName = null,
    string? Company = null,
    DateTimeOffset? DateOfDocument = null,
    string? Description = null
)
{
    public static DocumentUpdateModel CreateFromModel(DocumentModel model)
    {
        return new DocumentUpdateModel(model.CategoryName, model.DisplayName, model.Company, model.DateOfDocument,
            model.Description);
    }
    
    public static DocumentUpdateModel CreateFromEntity(Document doc)
    {
        return new DocumentUpdateModel(doc.Category?.DisplayName ?? string.Empty, doc.DisplayName, doc.Company,
            doc.DateOfDocument, doc.Description);
    }
}