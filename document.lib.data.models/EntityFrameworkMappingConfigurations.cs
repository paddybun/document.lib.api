using document.lib.data.entities;
using document.lib.data.models.Documents;
using Mapster;

namespace document.lib.data.models;

public static class EntityFrameworkMappingConfigurations
{
    public static void ConfigureMapsterMappings()
    {
        TypeAdapterConfig<DocumentSaveModel, Document>
            .NewConfig()
            .Ignore(
                dest => dest.Tags);

        TypeAdapterConfig<Document, DocumentSaveModel>
            .NewConfig()
            .Ignore(dest => dest.Tags);
    }
}