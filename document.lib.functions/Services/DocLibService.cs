using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using document.lib.functions.Constants;
using document.lib.functions.Helper;
using document.lib.functions.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;

namespace document.lib.functions.Services
{
    class DocLibService
    {
        private readonly BlobContainerClient _bcc;
        private readonly CosmosClient _cosmosClient;

        public DocLibService()
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsDocumentLibStorage");
            var containerName = Environment.GetEnvironmentVariable("DocumentContainerName");
            var cosmosConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsCosmos");
            _cosmosClient = new CosmosClient(cosmosConnectionString);
            _bcc = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<DocLibDocument1> CreateDocLibDocumentAsync(DocLibDocument1 doc)
        {
            doc.Validate();

            var db = _cosmosClient.GetDatabase(TableNames.Doclib);
            var docLibContainer = db.GetContainer(TableNames.Doclib);

            await CreateCategoryAsync(doc, docLibContainer);
            await CreateTagsAsync(doc, docLibContainer);

            var folder = await GetCurrentFolderAsync(docLibContainer) ?? await CreateFolderAsync(docLibContainer);
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
                await MoveBlob(doc, docLibContainer);
                await docLibContainer.UpsertItemAsync(folder);
            }

            var guid = Guid.NewGuid();
            var id = $"Document.{doc.Name}.{guid}";
            doc.Id = id;
            doc.Name = $"{doc.Name}_{guid}";
            doc.Unsorted = false;

            await docLibContainer.CreateItemAsync(doc, new PartitionKey(id));
            return doc;
        }

        private async Task CreateCategoryAsync(DocLibDocument1 doc, Container doclibContainer)
        {
            var id = $"Category.{doc.Category}";
            var query = new QueryDefinition("SELECT * FROM doclib dl WHERE dl.id = @id").WithParameter("@id", id);
            var entity = (await QueryHelper.ExecuteQueryAsync<DocLibCategory>(query, doclibContainer)).SingleOrDefault();
            if (entity == null)
            {
                var cat = new DocLibCategory
                {
                    Id = id,
                    Name = doc.Category,
                    Description = ""
                };
                await doclibContainer.CreateItemAsync(cat);
            }
        }

        private async Task CreateTagsAsync(DocLibDocument1 doc, Container doclibContainer)
        {
            foreach (var docTag in doc.Tags)
            {
                var id = $"Tag.{docTag}";
                var query = new QueryDefinition("SELECT * FROM doclib dl WHERE dl.id = @id").WithParameter("@id", id);
                var entity = (await QueryHelper.ExecuteQueryAsync<DocLibTag>(query, doclibContainer)).SingleOrDefault();
                if (entity == null)
                {
                    var lowercased = docTag.ToLower();
                    var tag = new DocLibTag
                    {
                        Id = id,
                        Name = lowercased,
                    };
                    await doclibContainer.CreateItemAsync(tag);
                }
            }
        }

        private async Task<DocLibFolder> GetCurrentFolderAsync(Container doclibContainer)
        {
            var query = new QueryDefinition("SELECT * FROM doclib dl where dl.IsFull = false");
            var folders = await QueryHelper.ExecuteQueryAsync<DocLibFolder>(query, doclibContainer);
            return folders.FirstOrDefault();
        }

        private async Task<DocLibFolder> CreateFolderAsync(Container doclibContainer)
        {
            var guid = Guid.NewGuid().ToString();
            var id = $"Folder.{guid}";
            var newFolder = new DocLibFolder
            {
                Id = id,
                Name = guid,
                CurrentRegister = "1",
                IsFull = false,
                TotalDocuments = 0,
                DisplayName = "Please choose a name",
                CreatedAt = DateTimeOffset.Now
            };
            newFolder.Registers.Add("1", 0);
            await doclibContainer.CreateItemAsync(newFolder);
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

        private async Task MoveBlob(DocLibDocument1 doc, Container doclibContainer)
        {
            var query = new QueryDefinition("SELECT * FROM doclib dl WHERE dl.id = @id")
                    .WithParameter("@id", doc.Id);
            var unsortedEntry1 = await QueryHelper.ExecuteQueryAsync<DocLibDocument1>(query, doclibContainer);
            var unsortedEntry = unsortedEntry1.First();
            if (unsortedEntry == null)
            {
                return;
            }
            await doclibContainer.DeleteItemAsync<DocLibDocument1>(unsortedEntry.Id, new PartitionKey(unsortedEntry.Id));

            var newBlobLocation = $"{doc.FolderName}/{doc.RegisterName}/{doc.PhysicalName}";
            var source = _bcc.GetBlobClient(doc.BlobLocation);
            if (await source.ExistsAsync())
            {
                var destBlob = _bcc.GetBlobClient(newBlobLocation);
                await destBlob.StartCopyFromUriAsync(source.Uri);
            }

            await source.DeleteAsync();
            doc.BlobLocation = newBlobLocation;
        }
    }
}
