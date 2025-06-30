using document.lib.bl.contracts.Folders;
using document.lib.bl.contracts.Upload;
using document.lib.bl.shared.Folders;
using document.lib.bl.shared.Upload;
using Microsoft.Extensions.DependencyInjection;

namespace document.lib.bl.shared;

public static class CqrsDependencyModule
{
    public static IServiceCollection AddBusinessShared(this IServiceCollection serviceCollection)
    {
        
        // Upload
        serviceCollection.AddTransient<IUploadBlobCommand, UploadBlobCommand>();
        serviceCollection.AddTransient<IAddToIndexCommand, AddToIndexCommand>();
        serviceCollection.AddTransient<IUploadBlobUseCase, UploadBlobUseCase>();
        serviceCollection.AddTransient<IDeleteBlobCommand, DeleteBlobCommand>();
        
        // Folders
        serviceCollection.AddTransient<IFolderQuery, FolderQuery>();
        serviceCollection.AddTransient<IFoldersQuery, FoldersQuery>();

        return serviceCollection;
    }
}