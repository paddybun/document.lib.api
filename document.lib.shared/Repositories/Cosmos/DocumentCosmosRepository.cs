using Azure.Storage.Blobs;
using document.lib.shared.Constants;
using document.lib.shared.Exceptions;
using document.lib.shared.Helper;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

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

    public async Task DeleteDocumentAsync(DocLibDocument doc)
    {
        await DeleteDocumentAsync(doc.Id);
    }

    public async Task DeleteDocumentAsync(string documentId)
    {
        var query = new QueryDefinition("SELECT * FROM doclib dl WHERE dl.id = @id").WithParameter("@id", documentId);
        var entity = (await CosmosQueryHelper.ExecuteQueryAsync<DocLibDocument>(query, _cosmosContainer)).SingleOrDefault();
        if (entity != null)
        {
            var storagePath = entity.BlobLocation;
            await _cosmosContainer.DeleteItemAsync<DocLibDocument>(documentId, new PartitionKey(documentId));

            var source = _bcc.GetBlobClient(storagePath);
            await source.DeleteAsync();
        }
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

    public Task<DocLibDocument> CreateDocumentAsync(DocLibDocument document)
    {
        throw new NotImplementedException();
    }

    public async Task<DocLibDocument> GetDocumentAsync(DocumentQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (string.IsNullOrWhiteSpace(queryParameters.Name)) 
            throw new InvalidQueryParameterException("Document query in cosmos repository only allows searching by name. Name should look like xyz.<Id as guid>");

        var id = queryParameters.Name.Split('.').Last();
        var document = _cosmosContainer.GetItemLinqQueryable<DocLibDocument>(true)
            .Where(x => x.Id.EndsWith(id))
            .AsEnumerable()
            .FirstOrDefault();

        return await Task.FromResult(document);
    }

    public async Task<List<DocLibDocument>> GetDocumentsAsync(int page, int count)
    {
        var documents = _cosmosContainer.GetItemLinqQueryable<DocLibDocument>(true)
            .Where(x => x.Id.StartsWith("Document."))
            .AsEnumerable()
            .ToList();

        return await Task.FromResult(documents);
    }

    public Task<int> GetDocumentCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<DocLibDocument>> GetDocumentsForFolderAsync(string folderName, int page, int count)
    {
        throw new NotImplementedException();
    }

    public Task<DocLibDocument> UpdateDocumentAsync(DocLibDocument document, DocLibCategory category, DocLibFolder folder, DocLibTag[] tags)
    {
        throw new NotImplementedException();
    }
}