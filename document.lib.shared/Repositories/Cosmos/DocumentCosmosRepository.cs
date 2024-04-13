using Azure.Storage.Blobs;
using document.lib.shared.Constants;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.Models;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace document.lib.shared.Repositories.Cosmos;

public class DocumentCosmosRepository : IDocumentRepository
{
    private readonly BlobContainerClient _bcc;
    private readonly Container _cosmosContainer;

    public DocumentCosmosRepository(IOptions<AppConfiguration> config)
    {
        _bcc = new BlobContainerClient(config.Value.BlobServiceConnectionString, config.Value.BlobContainer);
        var cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        var db = cosmosClient.GetDatabase(TableNames.Doclib);
        _cosmosContainer = db.GetContainer(TableNames.Doclib);
    }

    public async Task<List<DocumentModel>> GetUnsortedDocumentsAsync()
    {
        var docs = _cosmosContainer.GetItemLinqQueryable<DocLibDocument>(true)
            .Where(x => x.Unsorted == true)
            .AsAsyncEnumerable();
        var result = new List<DocumentModel>();
        await foreach (var doc in docs)
        {
            result.Add(Map(doc));
        }
        return result;
    }

    public Task DeleteDocumentAsync(DocumentModel doc)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDocumentAsync(string documentId)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateFolderReferenceAsync(string folderId, string folderDisplayName)
    {
        var documentsToUpdate = _cosmosContainer.GetItemLinqQueryable<DocLibDocument>(true)
            .Where(x => x.Id.StartsWith("Document"))
            .Where(x => x.FolderId == folderId)
            .AsEnumerable()
            .ToList();

        List<PatchOperation> operations = new()
        {
            PatchOperation.Replace("/folderName", folderDisplayName)
        };

        foreach (var document in documentsToUpdate)
        {
            await _cosmosContainer.PatchItemAsync<DocLibDocument>(document.Id, new PartitionKey(document.Id),
                operations);
        }
    }

    public Task<DocumentModel> CreateDocumentAsync(DocumentModel document)
    {
        throw new NotImplementedException();
    }

    public async Task<DocumentModel?> GetDocumentAsync(DocumentModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        if (string.IsNullOrWhiteSpace(model.Name)) 
            throw new InvalidParameterException("Document query in cosmos repository only allows searching by name. Name should look like xyz.<Id as guid>");

        var id = model.Name.Split('.').Last();
        var document = _cosmosContainer.GetItemLinqQueryable<DocLibDocument>(true)
            .Where(x => x.Id.EndsWith(id))
            .AsEnumerable()
            .FirstOrDefault();

        if (document == null) return null;

        return await Task.FromResult(Map(document));
    }

    public async Task<List<DocumentModel>> GetAllDocumentsAsync()
    {
        return await GetDocumentsPagedAsync(0, 0);
    }

    public async Task<List<DocumentModel>> GetDocumentsPagedAsync(int page, int count)
    {
        var documents = _cosmosContainer.GetItemLinqQueryable<DocLibDocument>(true)
            .Where(x => x.Id.StartsWith("Document."))
            .AsEnumerable()
            .ToList();

        var result = documents.Select(Map).ToList();

        return await Task.FromResult(result);
    }

    public Task<int> GetDocumentCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<DocumentModel>> GetDocumentsForFolderAsync(string folderName, int page, int count)
    {
        throw new NotImplementedException();
    }

    public Task<DocumentModel> UpdateDocumentAsync(DocumentModel document, CategoryModel? category, FolderModel? folder, TagModel[]? tags)
    {
        throw new NotImplementedException();
    }

    private static DocumentModel Map(DocLibDocument document)
    {
        return new DocumentModel
        {
            Id = document.Id,
            Name = document.Name,
            DisplayName = document.DisplayName,
            PhysicalName = document.PhysicalName,
            BlobLocation = document.BlobLocation,
            Company = document.Company,
            DateOfDocument = document.DateOfDocument,
            UploadDate = document.UploadDate,
            Description = document.Description,
            RegisterName = document.RegisterName,
            Tags = document.Tags.ToList(),
            Unsorted = document.Unsorted,
            CategoryId = "",
            CategoryName = document.CategoryName,
            Digital = document.DigitalOnly,
            FolderId = document.FolderId,
            FolderName = document.FolderName,
            DateModified = document.LastUpdate
        };
    }
}