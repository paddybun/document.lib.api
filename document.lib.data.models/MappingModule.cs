using Microsoft.Extensions.DependencyInjection;

namespace document.lib.data.models;

public static class MappingModule
{
    public static IServiceCollection AddModelMappings(this IServiceCollection services)
    {
        EntityFrameworkMappingConfigurations.ConfigureMapsterMappings();
        return services;
    }
}