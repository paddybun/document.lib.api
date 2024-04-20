using Azure.Storage.Blobs;
using document.lib.shared.Enums;
using document.lib.shared.Interfaces;
using document.lib.shared.Repositories.Cosmos;
using document.lib.shared.Repositories.Sql;
using document.lib.shared.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace document.lib.shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureDocumentLibShared(this IServiceCollection me, string databaseProvider, string cosmosDbConnection, string blobstorageConnectionString, string blobContainer)
    {
        if (me == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(databaseProvider)) throw new ArgumentNullException(nameof(databaseProvider));
        if (string.IsNullOrWhiteSpace(cosmosDbConnection)) throw new ArgumentNullException(nameof(cosmosDbConnection));
        if (string.IsNullOrWhiteSpace(blobstorageConnectionString)) throw new ArgumentNullException(nameof(blobstorageConnectionString));
        if (string.IsNullOrWhiteSpace(blobContainer)) throw new ArgumentNullException(nameof(blobContainer));

        var dbProvider = Enum.Parse<DatabaseProvider>(databaseProvider);

            // Repositories
        switch (dbProvider)
        {
            case DatabaseProvider.Sql:
                me.AddScoped<IDocumentRepository, DocumentSqlRepository>();
                me.AddScoped<ICategoryRepository, CategorySqlRepository>();
                me.AddScoped<ITagRepository, TagSqlRepository>();
                me.AddScoped<IFolderRepository, FolderSqlRepository>();
                me.AddScoped<IRegisterRepository, RegisterSqlRepository>();
                break;
            case DatabaseProvider.Cosmos:
                me.AddSingleton(new CosmosClient(cosmosDbConnection));
                me.AddScoped<IDocumentRepository, DocumentCosmosRepository>();
                me.AddScoped<ICategoryRepository, CategoryCosmosRepository>();
                me.AddScoped<ITagRepository, TagCosmosRepository>();
                me.AddScoped<IFolderRepository, FolderCosmosRepository>();
                me.AddScoped(typeof(CosmosQueryService));
                me.AddScoped(typeof(CosmosMetadataService));
                break;
        }

        // Services
        me.AddScoped<ICategoryService, CategoryService>();
        me.AddScoped<IDocumentService, DocumentService>();
        me.AddScoped<ITagService, TagService>();
        me.AddScoped<IFolderService, FolderService>();
        me.AddScoped<OneTimeSetup>();
        me.AddScoped<IndexerService>();

        me.AddSingleton<BlobClientHelper>();

        var blobContainerClient = new BlobContainerClient(blobstorageConnectionString, blobContainer);
        me.AddSingleton(blobContainerClient);
    }
}