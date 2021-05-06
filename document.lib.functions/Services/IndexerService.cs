using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using document.lib.functions.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.WindowsAzure.Storage.Blob;

namespace document.lib.functions.Services
{
    public class IndexerService
    {
        private readonly BlobContainerClient _bcc;
        private readonly CloudTableClient _tbs;

        public IndexerService()
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsDocumentLibStorage");
            var containerName = Environment.GetEnvironmentVariable("DocumentContainerName");
            _bcc = new BlobContainerClient(connectionString, containerName);
            var csa = CloudStorageAccount.Parse(connectionString);
            _tbs = csa.CreateCloudTableClient(new TableClientConfiguration());
        }

        public async Task<TableResult> IndexSingleDocumentAsync(ICloudBlob blob)
        {
            var tableName = "doclib";
            var table = _tbs.GetTableReference(tableName);
            var name = blob.Name.Split("/").Last();
            var doc = new DocLibDocument
            {
                Name = name,
                PhysicalName = blob.Name,
                BlobLocation = blob.Name,
                PartitionKey = "unsorted",
                RowKey = name,
                UploadDate = DateTimeOffset.Now,
                Timestamp = DateTimeOffset.Now,
                ETag = "*",
                Unsorted = true,
                Tags = ""
            };
            var op = TableOperation.InsertOrMerge(doc);
            var res = await table.ExecuteAsync(op);
            return res;
        }

        public async Task<TableBatchResult> IndexUnsortedAsync()
        {
            var tableName = "doclib";
            var table = _tbs.GetTableReference(tableName);
            var blobs = _bcc.GetBlobsAsync(prefix: "unsorted").GetAsyncEnumerator();
            var blobListing = new List<BlobItem>();
            while (await blobs.MoveNextAsync())
            {
                blobListing.Add(blobs.Current);
            }

            var batch = new TableBatchOperation();
            foreach (var blob in blobListing)
            {
                var name = blob.Name.Split("/").Last();
                var doc = new DocLibDocument
                {
                    Name = name,
                    PhysicalName = blob.Name,
                    BlobLocation = blob.Name,
                    PartitionKey = "unsorted",
                    RowKey = name,
                    UploadDate = blob.Properties.LastModified ?? DateTimeOffset.Now,
                    Timestamp = DateTimeOffset.Now,
                    ETag = "*",
                    Unsorted = true,
                    Tags = ""
                };
                batch.InsertOrMerge(doc);
            }
            var result = await table.ExecuteBatchAsync(batch);
            return result;
        }
    }
}
