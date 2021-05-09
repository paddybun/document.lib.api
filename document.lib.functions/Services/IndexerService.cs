using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using document.lib.functions.Constants;
using document.lib.functions.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.WindowsAzure.Storage.Blob;

namespace document.lib.functions.Services
{
    public class IndexerService
    {
        private readonly BlobContainerClient _bcc;
        private readonly CosmosClient _cosmosClient;

        public IndexerService()
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsDocumentLibStorage");
            var containerName = Environment.GetEnvironmentVariable("DocumentContainerName");
            _bcc = new BlobContainerClient(connectionString, containerName);
            var cosmosConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsCosmos");
            _cosmosClient = new CosmosClient(cosmosConnectionString);
        }

        public async Task IndexSingleDocumentAsync(ICloudBlob blob)
        {
            var db = _cosmosClient.GetDatabase(TableNames.Doclib);
            var container = db.GetContainer(TableNames.Doclib);
            var name = blob.Name.Split("/").Last();

            var id = $"UnsortedDocument.{name}";
            var doc = new DocLibDocument1
            {
                Id = id,
                Name = name,
                PhysicalName = blob.Name,
                BlobLocation = blob.Name,
                UploadDate = DateTimeOffset.Now,
                Unsorted = true,
                Tags = new string[0]
            };

            var result = await container.CreateItemAsync(doc, new PartitionKey(id));
        }
    }
}
