using Azure.Storage.Blobs;
using document.lib.shared.Constants;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;

namespace document.lib.shared.Services
{
    public class IndexerService
    {
        private readonly CosmosClient _cosmosClient;

        public IndexerService()
        {
            var cosmosConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsCosmos");
            _cosmosClient = new CosmosClient(cosmosConnectionString);
        }

        public async Task IndexSingleDocumentAsync(BlobClient blob)
        {
            var db = _cosmosClient.GetDatabase(TableNames.Doclib);
            var container = db.GetContainer(TableNames.Doclib);
            var name = blob.Name.Split("/").Last();

            var id = $"UnsortedDocument.{name}";
            var now = DateTimeOffset.Now;
            var doc = new DocLibDocument
            {
                Id = id,
                Name = name,
                PhysicalName = blob.Name,
                BlobLocation = blob.Name,
                UploadDate = now,
                LastUpdate = now,
                Unsorted = true,
                Tags = Array.Empty<string>()
            };

            await container.CreateItemAsync(doc, new PartitionKey(id));
        }
    }
}
