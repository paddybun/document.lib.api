using document.lib.bl.contracts.Categories;
using document.lib.bl.contracts.DocumentHandling;
using document.lib.bl.contracts.Folders;
using document.lib.bl.contracts.Upload;
using document.lib.bl.shared.Categories;
using document.lib.bl.shared.DocumentHandling;
using document.lib.bl.shared.Folders;
using document.lib.bl.shared.Upload;
using Microsoft.Extensions.DependencyInjection;

namespace document.lib.bl.shared;

public static class CqrsDependencyModule
{
    public static IServiceCollection AddBusinessShared(this IServiceCollection serviceCollection)
    {
        // Categories
        serviceCollection.AddTransient<ICategoryQuery<UnitOfWork>, CategoryQuery>();
        serviceCollection.AddTransient<ICategoriesQuery<UnitOfWork>, CategoriesQuery>();

        // Upload
        serviceCollection.AddTransient<IUploadBlobCommand, UploadBlobCommand>();
        serviceCollection.AddTransient<IAddToIndexCommand, AddToIndexCommand>();
        serviceCollection.AddTransient<IUploadBlobUseCase, UploadBlobUseCase>();
        serviceCollection.AddTransient<IDeleteBlobCommand, DeleteBlobCommand>();
        
        // Folders
        serviceCollection.AddTransient<IFolderQuery<UnitOfWork>, FolderQuery>();
        serviceCollection.AddTransient<IFoldersQuery<UnitOfWork>, FoldersQuery>();
        serviceCollection.AddTransient<IGetRegisterUseCase<UnitOfWork>, GetRegisterUseCase>();
        serviceCollection.AddTransient<INextDescriptionQuery<UnitOfWork>, NextDescriptionQuery>();
        serviceCollection.AddTransient<IGetFolderOverviewUseCase<UnitOfWork>, GetFolderOverviewUseCase>();
        serviceCollection.AddTransient<ISaveFolderUseCase<UnitOfWork>, SaveFolderUseCase>();
        serviceCollection.AddTransient<IDeleteFolderUseCase<UnitOfWork>, DeleteFolderUseCase>();
        serviceCollection.AddTransient<IActivateFolderUseCase<UnitOfWork>, ActivateFolderUseCase>();
        
        // Descriptions
        serviceCollection.AddTransient<IRegisterDescriptionsQuery<UnitOfWork>, RegisterDescriptionsQuery>();
        serviceCollection.AddTransient<IRegisterDescriptionQuery<UnitOfWork>, RegisterDescriptionQuery>();
        
        
        return serviceCollection;
    }
}