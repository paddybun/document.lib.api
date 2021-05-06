using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using document.lib.functions.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Azure.Documents.Linq;

namespace document.lib.functions.Services
{
    class DocLibService
    {
        private BlobContainerClient _bcc;
        private CloudTableClient _tbs;

        public DocLibService()
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsDocumentLibStorage");
            var containerName = Environment.GetEnvironmentVariable("DocumentContainerName");
            var csa = CloudStorageAccount.Parse(connectionString);

            _bcc = new BlobContainerClient(connectionString, containerName);
            _tbs = csa.CreateCloudTableClient(new TableClientConfiguration());
        }

        public async Task<DocLibDocument> CreateDocLibDocumentAsync(DocLibDocument doc)
        {
            doc.Validate();
            var partitionKey = "document";
            var documentTableName = "doclib";
            var metadataTableName = "metadata";

            var doclibTable = _tbs.GetTableReference(documentTableName);
            var metadataTable = _tbs.GetTableReference(metadataTableName);

            await doclibTable.CreateIfNotExistsAsync();
            await metadataTable.CreateIfNotExistsAsync();

            await CreateCategoryAsync(doc, metadataTable);
            await CreateTagsAsync(doc, metadataTable);

            var folder = GetCurrentFolder(metadataTable) ?? await CreateFolder(metadataTable);
            var register = GetRegister(folder);
            if (string.IsNullOrEmpty(doc.FolderName) && string.IsNullOrEmpty(doc.RegisterName))
            {
                doc.FolderName = folder.Name;
                doc.RegisterName = register;
                folder.TotalDocuments += 1;
                if (folder.TotalDocuments >= folder.DocumentsPerFolder)
                {
                    folder.IsFull = true;
                }
                await MoveBlob(doc, doclibTable);
                var folderOp = TableOperation.InsertOrMerge(folder);
                await metadataTable.ExecuteAsync(folderOp);
            }

            var guid = Guid.NewGuid();
            doc.Name = $"{doc.Name}_{guid}";
            doc.PartitionKey = partitionKey;
            doc.RowKey = doc.Name;
            doc.ETag = "*";
            doc.Timestamp = DateTimeOffset.Now;
            doc.Unsorted = false;

            var op = TableOperation.InsertOrMerge(doc);
            await doclibTable.ExecuteAsync(op);

            return doc;
        }

        private async Task CreateCategoryAsync(DocLibDocument doc, CloudTable metadataTable)
        {
            var partitionKey = "category";
            var entity = new DocLibCategory
            {
                Name = doc.Category,
                Description = "",
                ETag = "*",
                Timestamp = DateTimeOffset.Now,
                PartitionKey = partitionKey,
                RowKey = doc.Category.ToLower()
            };
            var op = TableOperation.InsertOrReplace(entity);
            var res = await metadataTable.ExecuteAsync(op);
        }

        private async Task CreateTagsAsync(DocLibDocument doc, CloudTable metadataTable)
        {
            var partitionKey = "tag";
            var now = DateTimeOffset.Now;

            var batch = new TableBatchOperation();
            var tagString = doc.Tags.Remove(0, 1);
            tagString = tagString.Remove(tagString.Length - 1, 1);
            var tags = tagString.Split("|");

            foreach (var docTag in tags)
            {
                var lowercased = docTag.ToLower();
                batch.InsertOrReplace(new DocLibTag
                {
                    Name = lowercased,
                    Timestamp = now,
                    ETag = "*",
                    PartitionKey = partitionKey,
                    RowKey = lowercased
                });
            }

            await metadataTable.ExecuteBatchAsync(batch);
        }

        private DocLibFolder GetCurrentFolder(CloudTable metadataTable)
        {
            var partitionKey = "folder";
            var query = metadataTable.CreateQuery<DocLibFolder>().Where(x => x.PartitionKey == partitionKey && !x.IsFull).AsTableQuery();
            var folder = metadataTable.ExecuteQuery(query).FirstOrDefault();
            return folder;
        }

        private async Task<DocLibFolder> CreateFolder(CloudTable metadataTable)
        {
            var partitionKey = "folder";
            var guid = Guid.NewGuid();
            var newFolder = new DocLibFolder
            {
                Name = guid.ToString(),
                CurrentRegister = "1",
                IsFull = false,
                TotalDocuments = 0,
                DisplayName = "Please choose a name",
                Timestamp = DateTimeOffset.Now,
                CreatedAt = DateTimeOffset.Now,
                RowKey = guid.ToString(),
                PartitionKey = partitionKey,
                ETag = "*"
            };
            newFolder.Registers.Add("1", 0);
            var op = TableOperation.Insert(newFolder);
            await metadataTable.ExecuteAsync(op);
            return newFolder;
        }

        private string GetRegister(DocLibFolder folder)
        {
            var register = folder.Registers[folder.CurrentRegister];
            if (register >= 10)
            {
                var registerIndex = (Convert.ToInt32(folder.CurrentRegister) + 1).ToString();
                folder.Registers.Add(registerIndex, 1);
                folder.CurrentRegister = registerIndex;
                return registerIndex;
            }

            folder.Registers[folder.CurrentRegister] += 1;
            return folder.CurrentRegister;
        }

        private async Task MoveBlob(DocLibDocument doc, CloudTable doclibTable)
        {
            var partitionKey = "unsorted";
            var unsortedQuery = doclibTable.CreateQuery<DocLibDocument>().Where(x => x.PartitionKey == partitionKey && x.RowKey == doc.PhysicalName).AsTableQuery();
            var unsortedEntry = doclibTable.ExecuteQuery(unsortedQuery).SingleOrDefault();
            if (unsortedEntry == null)
            {
                return;
            }
            unsortedEntry.Unsorted = false;
            var updateEntry = TableOperation.InsertOrMerge(unsortedEntry);
            await doclibTable.ExecuteAsync(updateEntry);

            var newBlobLocation = $"{doc.FolderName}/{doc.RegisterName}/{doc.PhysicalName}";
            var source = _bcc.GetBlobClient(doc.BlobLocation);
            if (await source.ExistsAsync())
            {
                var destBlob = _bcc.GetBlobClient(newBlobLocation);
                await destBlob.StartCopyFromUriAsync(source.Uri);
            }

            doc.BlobLocation = newBlobLocation;
        }
    }
}
