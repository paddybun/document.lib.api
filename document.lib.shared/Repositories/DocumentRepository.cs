using Azure.Storage.Blobs;
using document.lib.shared.Constants;
using document.lib.shared.Helper;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly BlobContainerClient _bcc;
    private readonly Container _cosmosContainer;

    public DocumentRepository(IOptions<AppConfiguration> config)
    {
        _bcc = new BlobContainerClient(config.Value.BlobContainerConnectionString, config.Value.BlobContainer);
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
}