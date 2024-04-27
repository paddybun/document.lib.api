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
        var sqlCategoryService = new CategoryService(new CategorySqlRepository(sqlContext));
        var sqlFolderService = new FolderService(new FolderSqlRepository(sqlContext));
        var sqlTagService = new TagService(new TagSqlRepository(sqlContext));
        var sqlRegisterRepo = new RegisterSqlRepository(sqlContext);
        var sqlDocumentService = new DocumentService(blobContainerClient, new DocumentSqlRepository(sqlContext), sqlCategoryService, sqlTagService, sqlFolderService);

        await SyncCategoriesAsync(sqlCategoryService);
        await SyncFoldersAsync(sqlFolderService);
        await SyncTagsAsync(sqlTagService);
        await SyncRegistersAsync(sqlRegisterRepo);
        await SyncDocumentsAsync(sqlDocumentService);

        logger.LogInformation("Cosmos to sql migration successful");
    }

    private async Task SyncDocumentsAsync(DocumentService sqlDocumentService)
    {
        logger.LogInformation("Synchronizing documents from Cosmos to Sql ...");
        var sw = Stopwatch.StartNew();
        List<DocumentModel> docs = []; // await cosmosDocumentService.GetAllDocumentsAsync(); TODO: Use paged method
        foreach (var doc in docs)
        {
            await sqlDocumentService.CreateNewDocumentAsync(doc);
        }
        sw.Stop();
        logger.LogInformation("Synchronizing documents done. {count} entities inserted in {time} ms", docs.Count, sw.ElapsedMilliseconds);
    }

    private async Task SyncRegistersAsync(RegisterSqlRepository registerRepo)
    {
        logger.LogInformation("Synchronizing registers from Cosmos to Sql ...");
        var sw = Stopwatch.StartNew();
        var folders = await cosmosFolderService.GetAllAsync();
        var toReturn = new List<RegisterModel>();

        foreach (var folder in folders)
        {
            foreach (var register in folder.Registers)
            {
                toReturn.Add(await registerRepo.CreateRegistersAsync(register));
            }
        }
        
        var unsortedRegister = new RegisterModel
        {
            Name = "unsorted",
            DisplayName = "unsorted",
            DocumentCount = 0
        };
        var digitalRegister = new RegisterModel
        {
            Name = "digital",
            DisplayName = "digital",
            DocumentCount = 0
        };

        await registerRepo.CreateRegistersAsync(unsortedRegister);
        await registerRepo.CreateRegistersAsync(digitalRegister);

        sw.Stop();
        logger.LogInformation("Synchronizing registers done. {count} entities inserted in {time} ms", toReturn.Count, sw.ElapsedMilliseconds);
    }

    private async Task SyncTagsAsync(TagService tagService)
    {
        logger.LogInformation("Synchronizing tags from Cosmos to Sql ...");
        var sw = Stopwatch.StartNew();
        var tags = await cosmosTagService.GetTagsAsync();
        var newTags = await tagService.GetOrCreateTagsAsync(tags.Select(x => x.Name).ToList());
        sw.Stop();
        logger.LogInformation("Synchronizing tags done. {count} entities inserted in {time} ms", tags.Count, sw.ElapsedMilliseconds);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Hosted service will be stopped now");
        await Task.CompletedTask;
    }

    private async Task SyncCategoriesAsync(ICategoryService service)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("Synchronizing categories from Cosmos to Sql ...");
        var categories = await cosmosCategoryService.GetAllAsync();
        var newCategories = new List<CategoryModel>();
        foreach (var efCategory in categories)
        {
            newCategories.Add(await service.SaveAsync(efCategory, true));
        }
        var uncategorized = new CategoryModel
        {
            Name = "uncategorized",
            DisplayName = "uncategorized",
            Description = "uncategorized"
        };
        await service.SaveAsync(uncategorized, true);
        sw.Stop();
        logger.LogInformation("Synchronizing categories done. {count} entities inserted in {time} ms", categories.Count, sw.ElapsedMilliseconds);
    }

    private async Task SyncFoldersAsync(IFolderService service)
    {
        var sw = Stopwatch.StartNew();
        logger.LogInformation("Synchronizing folders from Cosmos to Sql ...");
        var folders = await cosmosFolderService.GetAllAsync();
        var newFolders = new List<FolderModel>();
        foreach (var folder in folders)
        {
            newFolders.Add(await service.SaveAsync(folder, true));
        }
        sw.Stop();
        logger.LogInformation("Synchronizing folders done. {count} entities inserted in {time} ms", folders.Count, sw.ElapsedMilliseconds);
    }
}