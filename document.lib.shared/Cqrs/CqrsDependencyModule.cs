using document.lib.shared.Cqrs.Interfaces;
using document.lib.shared.Cqrs.Upload;
using Microsoft.Extensions.DependencyInjection;

namespace document.lib.shared.Cqrs;

public static class CqrsDependencyModule
{
    public static IServiceCollection AddCqrs(this IServiceCollection serviceCollection)
    {
        
        // Upload
        serviceCollection.AddTransient<IUploadBlobCommand, UploadBlobCommand>();
        serviceCollection.AddTransient<IAddToIndexCommand, AddToIndexCommand>();
        serviceCollection.AddTransient<IUploadBlobUseCase, UploadBlobUseCase>();

        return serviceCollection;
    }
}