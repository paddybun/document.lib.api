using System.Diagnostics;
using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.ViewModels;
using document.lib.shared.Repositories.Cosmos;
using document.lib.shared.Repositories.Sql;
using document.lib.shared.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace document.lib.migrationtool;

public class CosmosToSqlMigration: IHostedService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<CosmosToSqlMigration> _logger;
    private readonly IFolderService _cosmosFolderService;
    private readonly IDocumentService _cosmosDocumentService;
    private readonly ICategoryService _cosmosCategoryService;
    private readonly DocumentLibContext _sqlContext;

    public CosmosToSqlMigration(IHostApplicationLifetime lifetime,
        ILogger<CosmosToSqlMigration> logger,
        IFolderService cosmosFolderService,
        IDocumentService cosmosDocumentService,
        ICategoryService cosmosCategoryService,
        DocumentLibContext sqlContext)
    {
        _lifetime = lifetime;
        _logger = logger;
        _cosmosFolderService = cosmosFolderService;
        _cosmosDocumentService = cosmosDocumentService;
        _cosmosCategoryService = cosmosCategoryService;
        _sqlContext = sqlContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // allow generic host to log lifetime logs first
        var sw = new Stopwatch();
        _logger.LogInformation("Started cosmos to sql migration");
        var sqlCategoryService = new CategoryService(new CategorySqlRepository(_sqlContext));
        var sqlFolderService = new FolderService(new FolderSqlRepository(_sqlContext), new DocumentSqlRepository(_sqlContext));

        _logger.LogInformation("Synchronizing categories from Cosmos to Sql ...");
        sw.Start();
        var categories = await SyncCategoriesAsync(sqlCategoryService);
        sw.Stop();
        _logger.LogInformation("Synchronizing categories done. {count} entities inserted in {time} ms", categories.Count, sw.ElapsedMilliseconds);

        _logger.LogInformation("Synchronizing folders from Cosmos to Sql ...");
        sw.Start();
        var folders = await SyncFoldersAsync(sqlFolderService);
        sw.Stop();
        _logger.LogInformation("Synchronizing folders done. {count} entities inserted in {time} ms", folders.Count, sw.ElapsedMilliseconds);

        //var folders = await _folderService.GetAllAsync();
        //var docs = await _documentService.GetAllDocumentsAsync();
        //var docsGroupedByRegister = docs.GroupBy(x => x.RegisterName);
        //var efRegister = new EfRegister();


        _logger.LogInformation("Cosmos to sql migration successful");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted service will be stopped now");
        await Task.CompletedTask;
    }

    private async Task<List<CategoryModel>> SyncCategoriesAsync(ICategoryService service)
    {
        var categories = await _cosmosCategoryService.GetAllAsync();
        var newCategories = new List<CategoryModel>();
        foreach (var efCategory in categories)
        {
            newCategories.Add(await service.SaveAsync(efCategory, true));
        }
        return newCategories;
    }

    private async Task<List<FolderModel>> SyncFoldersAsync(IFolderService service)
    {
        var folders = await _cosmosFolderService.GetAllAsync();
        var newFolders = new List<FolderModel>();
        foreach (var folder in folders)
        {
            newFolders.Add(await service.SaveAsync(folder, true));
        }

        return newFolders;
    }

    private EfDocument CreateDocument(DocumentModel documentModel)
    {
        return new EfDocument
        {
            DateCreated = DateTimeOffset.Now,
            DateModified = documentModel.DateModified,
            Name = documentModel.Name,
            DisplayName = documentModel.DisplayName,
            PhysicalName = documentModel.PhysicalName,
            BlobLocation = documentModel.BlobLocation,
            Company = documentModel.Company,
            DateOfDocument = documentModel.DateOfDocument,
            UploadDate = documentModel.UploadDate,
            Description = documentModel.Description,
            Unsorted = documentModel.Unsorted,
            Digital = documentModel.Digital
        };
    }
}