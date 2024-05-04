using System.Diagnostics;
using Azure.Storage.Blobs;
using document.lib.ef;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using document.lib.shared.Repositories.Sql;
using document.lib.shared.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace document.lib.migrationtool;

public class CosmosToSqlMigration(
    ILogger<CosmosToSqlMigration> logger,
    IFolderService cosmosFolderService,
    IDocumentService cosmosDocumentService,
    ICategoryService cosmosCategoryService,
    ITagService cosmosTagService,
    BlobContainerClient blobContainerClient,
    DocumentLibContext sqlContext)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // allow generic host to log lifetime logs first
        logger.LogInformation("Started cosmos to sql migration");
        // var documentSqlRepository = new DocumentSqlRepository(sqlContext);
        // var sqlCategoryService = new CategorySqlService(new CategorySqlRepository(sqlContext));
        // var sqlFolderService = new FolderSqlService(new FolderSqlRepository(sqlContext));
        // var sqlTagService = new TagSqlService(new TagSqlRepository(sqlContext));
        // var sqlDocumentService = new DocumentSqlService(blobContainerClient, documentSqlRepository, sqlCategoryService, sqlTagService, sqlFolderService);

        // await SyncCategoriesAsync(sqlCategoryService);
        // await SyncFoldersAsync(sqlFolderService);
        // await SyncTagsAsync(sqlTagService);
        // await SyncDocumentsAsync(sqlDocumentService);

        logger.LogInformation("Cosmos to sql migration successful");
    }

    private async Task SyncDocumentsAsync(DocumentSqlService sqlDocumentSqlService)
    {
        throw new NotImplementedException();
    }

    private async Task SyncTagsAsync(TagSqlService tagSqlService)
    {
        throw new NotImplementedException();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task SyncCategoriesAsync(ICategoryService service)
    {
        throw new NotImplementedException();
    }

    private async Task SyncFoldersAsync(IFolderService service)
    {
        throw new NotImplementedException();
    }
}