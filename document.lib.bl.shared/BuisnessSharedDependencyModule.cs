using document.lib.bl.contracts.Categories.Commands;
using document.lib.bl.contracts.Categories.Queries;
using document.lib.bl.contracts.Categories.UseCases;
using document.lib.bl.contracts.DocumentHandling.Queries;
using document.lib.bl.contracts.DocumentHandling.UseCases;
using document.lib.bl.contracts.Documents.Commands;
using document.lib.bl.contracts.Documents.Queries;
using document.lib.bl.contracts.Documents.UseCases;
using document.lib.bl.contracts.Folders.Queries;
using document.lib.bl.contracts.Folders.UseCases;
using document.lib.bl.contracts.RegisterDescriptions.Commands;
using document.lib.bl.contracts.RegisterDescriptions.Queries;
using document.lib.bl.contracts.RegisterDescriptions.UseCases;
using document.lib.bl.contracts.Tags.Commands;
using document.lib.bl.contracts.Tags.Queries;
using document.lib.bl.contracts.Tags.UseCases;
using document.lib.bl.contracts.Upload.Commands;
using document.lib.bl.contracts.Upload.UseCases;
using document.lib.bl.shared.Categories.Commands;
using document.lib.bl.shared.Categories.Queries;
using document.lib.bl.shared.Categories.UseCases;
using document.lib.bl.shared.DocumentHandling.Queries;
using document.lib.bl.shared.DocumentHandling.UseCases;
using document.lib.bl.shared.Documents.Commands;
using document.lib.bl.shared.Documents.Queries;
using document.lib.bl.shared.Documents.UseCases;
using document.lib.bl.shared.Folders.Queries;
using document.lib.bl.shared.Folders.UseCases;
using document.lib.bl.shared.RegisterDescriptions.Commands;
using document.lib.bl.shared.RegisterDescriptions.Queries;
using document.lib.bl.shared.RegisterDescriptions.UseCases;
using document.lib.bl.shared.Tags.Commands;
using document.lib.bl.shared.Tags.Queries;
using document.lib.bl.shared.Tags.UseCases;
using document.lib.bl.shared.Upload.Commands;
using document.lib.bl.shared.Upload.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace document.lib.bl.shared;

public static class CqrsDependencyModule
{
    public static IServiceCollection AddBusinessShared(this IServiceCollection serviceCollection)
    {
        // Categories
        serviceCollection.AddTransient<ICategoryQuery<UnitOfWork>, CategoryQuery>();
        serviceCollection.AddTransient<ICategoriesQuery<UnitOfWork>, CategoriesQuery>();
        serviceCollection.AddTransient<IGetCategoryUseCase<UnitOfWork>, GetCategoryUseCase>();
        serviceCollection.AddTransient<IGetCategoriesUseCase<UnitOfWork>, GetCategoriesUseCase>();
        serviceCollection.AddTransient<ICategoryOverviewQuery<UnitOfWork>, CategoryOverviewQuery>();
        serviceCollection.AddTransient<IUpdateCategoryCommand<UnitOfWork>, UpdateCategoryCommand>();
        serviceCollection.AddTransient<IGetCategoryOverviewUseCase<UnitOfWork>, GetCategoryOverviewUseCase>();
        serviceCollection.AddTransient<IUpdateCategoryUseCase<UnitOfWork>, UpdateCategoryUseCase>();
        serviceCollection.AddScoped<ICreateCategoryCommand<UnitOfWork>, CreateCategoryCommand>();
        serviceCollection.AddScoped<ICreateCategoryUseCase<UnitOfWork>, CreateCategoryUseCase>();
        serviceCollection.AddTransient<IMergeCategoryCommand<UnitOfWork>, MergeCategoryCommand>();
        serviceCollection.AddTransient<IDeleteCategoryCommand<UnitOfWork>, DeleteCategoryCommand>();
        serviceCollection.AddTransient<IMergeCategoryUseCase<UnitOfWork>, MergeCategoryUseCase>();
        serviceCollection.AddTransient<IDeleteCategoryUseCase<UnitOfWork>, DeleteCategoryUseCase>();

        // Upload
        serviceCollection.AddTransient<IUploadBlobCommand, UploadBlobCommand>();
        serviceCollection.AddTransient<IAddToIndexCommand, AddToIndexCommand>();
        serviceCollection.AddTransient<IUploadBlobUseCase, UploadBlobUseCase>();
        serviceCollection.AddTransient<IDeleteBlobCommand, DeleteBlobCommand>();
        serviceCollection.AddTransient<ICopyBlobCommand, CopyBlobCommand>();
        serviceCollection.AddTransient<IDownloadBlobCommand, DownloadBlobCommand>();
        
        // Documents
        serviceCollection.AddTransient<IDocumentListUseCase<UnitOfWork>, DocumentListUseCase>();
        serviceCollection.AddTransient<IDocumentOverviewQuery<UnitOfWork>, DocumentOverviewQuery>();
        serviceCollection.AddTransient<IDocumentQuery<UnitOfWork>, DocumentQuery>();
        serviceCollection.AddTransient<IGetDocumentUseCase<UnitOfWork>, GetDocumentUseCase>();
        serviceCollection.AddTransient<ISaveDocumentUseCase<UnitOfWork>, SaveDocumentUseCase>();
        serviceCollection.AddTransient<IGetDocumentFileUseCase<UnitOfWork>, GetDocumentFileUseCase>();
        serviceCollection.AddTransient<IMoveDocumentCommand<UnitOfWork>, MoveDocumentCommand>();
        serviceCollection.AddTransient<IDeleteDocumentCommand<UnitOfWork>, DeleteDocumentCommand>();
        serviceCollection.AddTransient<IDeleteDocumentUseCase<UnitOfWork>, DeleteDocumentUseCase>();
        serviceCollection.AddTransient<ICompaniesQuery<UnitOfWork>, CompaniesQuery>();
        serviceCollection.AddTransient<IGetCompaniesUseCase<UnitOfWork>, GetCompaniesUseCase>();
        serviceCollection.AddTransient<IUpdateDocumentCommand<UnitOfWork>, UpdateDocumentCommand>();
        
        // Folders
        serviceCollection.AddTransient<IFolderQuery<UnitOfWork>, FolderQuery>();
        serviceCollection.AddTransient<IFoldersQuery<UnitOfWork>, FoldersQuery>();
        serviceCollection.AddTransient<IActiveFolderQuery<UnitOfWork>, ActiveFolderQuery>();
        serviceCollection.AddTransient<IGetRegisterUseCase<UnitOfWork>, GetRegisterUseCase>();
        serviceCollection.AddTransient<INextDescriptionQuery<UnitOfWork>, NextDescriptionQuery>();
        serviceCollection.AddTransient<IGetFolderOverviewUseCase<UnitOfWork>, GetFolderOverviewUseCase>();
        serviceCollection.AddTransient<ISaveFolderUseCase<UnitOfWork>, SaveFolderUseCase>();
        serviceCollection.AddTransient<IDeleteFolderUseCase<UnitOfWork>, DeleteFolderUseCase>();
        serviceCollection.AddTransient<IActivateFolderUseCase<UnitOfWork>, ActivateFolderUseCase>();
        
        // Descriptions
        serviceCollection.AddTransient<IRegisterDescriptionsQuery<UnitOfWork>, RegisterDescriptionsQuery>();
        serviceCollection.AddTransient<IRegisterDescriptionQuery<UnitOfWork>, RegisterDescriptionQuery>();
        serviceCollection.AddTransient<IRegisterDescriptionAddCommand<UnitOfWork>, RegisterDescriptionAddCommand>();
        serviceCollection.AddTransient<IRegisterDescriptionSaveUseCase<UnitOfWork>, RegisterDescriptionSaveUseCase>();
        serviceCollection.AddTransient<IRegisterDescriptionRenameGroupCommand<UnitOfWork>, RegisterDescriptionRenameGroupCommand>();
        serviceCollection.AddTransient<IRegisterDescriptionUpdateCommand<UnitOfWork>, RegisterDescriptionUpdateCommand>();
        
        // Tags
        serviceCollection.AddTransient<ITagsQuery<UnitOfWork>, TagsQuery>();
        serviceCollection.AddTransient<IGetTagsUseCase<UnitOfWork>, GetTagsUseCase>();
        serviceCollection.AddTransient<ITagOverviewQuery<UnitOfWork>, TagOverviewQuery>();
        serviceCollection.AddTransient<IMergeTagCommand<UnitOfWork>, MergeTagCommand>();
        serviceCollection.AddTransient<IGetTagOverviewUseCase<UnitOfWork>, GetTagOverviewUseCase>();
        serviceCollection.AddTransient<IMergeTagUseCase<UnitOfWork>, MergeTagUseCase>();
        
        return serviceCollection;
    }
}