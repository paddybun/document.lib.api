using Azure.Storage.Blobs;
using document.lib.ef.Entities;
using document.lib.shared.Enums;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Repositories.Cosmos;
using document.lib.shared.Repositories.Sql;
using document.lib.shared.Services;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace document.lib.shared.Extensions;

public static class ServiceCollectionExtensions
{
    
    public static void UseDocumentLibShared(this IServiceCollection services, IConfigurationSection? configSection)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configSection);
        
        var appConfig = configSection.Get<SharedConfig>();
        if (appConfig == null)
        {
            throw new Exception("Config section not found!");
        }
        
        ArgumentNullException.ThrowIfNull(appConfig.DatabaseProvider);
        ArgumentException.ThrowIfNullOrEmpty(appConfig.CosmosDbConnection, nameof(appConfig.CosmosDbConnection));
        ArgumentException.ThrowIfNullOrEmpty(appConfig.BlobServiceConnectionString, nameof(appConfig.BlobServiceConnectionString));
        ArgumentException.ThrowIfNullOrEmpty(appConfig.BlobContainer, nameof(appConfig.BlobContainer));

        
        // Repositories
        switch (appConfig.DatabaseProvider)
        {
            case DatabaseProvider.Sql:
                services.AddScoped<IDocumentRepository<EfDocument>, DocumentSqlRepository>();
                services.AddScoped<ICategoryRepository<EfCategory>, CategorySqlRepository>();
                services.AddScoped<ITagRepository<EfTag>, TagSqlRepository>();
                services.AddScoped<IFolderRepository<EfFolder>, FolderSqlSqlRepository>();
                services.AddScoped<IDocumentService, DocumentSqlService>();
                services.AddScoped<ICategoryService, CategorySqlService>();
                services.AddScoped<IFolderService, FolderSqlService>();
                break;
            case DatabaseProvider.Cosmos:
                services.AddSingleton(new CosmosClient(appConfig.CosmosDbConnection));
                services.AddScoped<IDocumentRepository<DocLibDocument>, DocumentCosmosRepository>();
                services.AddScoped<ICategoryRepository<DocLibCategory>, CategoryCosmosRepository>();
                services.AddScoped<ITagRepository<DocLibTag>, TagCosmosRepository>();
                services.AddScoped<IFolderRepository<DocLibFolder>, FolderCosmosRepository>();
                services.AddScoped(typeof(CosmosQueryService));
                services.AddScoped(typeof(CosmosMetadataService));
                break;
            default:
                throw new Exception("Database provider not supported!");
        }

        // Services
        services.AddScoped<ITagService, TagSqlService>();
        services.AddScoped<IndexerService>();

        var blobContainerClient = new BlobContainerClient(appConfig.BlobServiceConnectionString, appConfig.BlobContainer);
        services.AddSingleton(blobContainerClient);
        services.AddSingleton(appConfig!);
        services.AddSingleton<BlobClientHelper>();
    }
}