using Azure.Storage.Blobs;
using document.lib.shared.Constants;
using document.lib.shared.Helper;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;

namespace document.lib.shared.Services
{
    public class DocumentService
    {
        private readonly BlobContainerClient _bcc;
        private readonly CosmosClient _cosmosClient;

        public DocumentService(string blobContainerConnectionString, string container, string cosmosConnectionString)
        {
            _bcc = new BlobContainerClient(blobContainerConnectionString, container);
            _cosmosClient = new CosmosClient(cosmosConnectionString);
        }

        public async Task DeleteDocumentAsync(DocLibDocument doc)
        {
            if (string.IsNullOrEmpty(doc.Id))
            {
                return;
            }
            var db = _cosmosClient.GetDatabase(TableNames.Doclib);
            var docLibContainer = db.GetContainer(TableNames.Doclib);
            var query = new QueryDefinition("SELECT * FROM doclib dl WHERE dl.id = @id").WithParameter("@id", doc.Id);
            var entity = (await CosmosQueryHelper.ExecuteQueryAsync<DocLibDocument>(query, docLibContainer)).SingleOrDefault();
            if (entity != null)
            {
                var storagePath = entity.BlobLocation;
                await docLibContainer.DeleteItemAsync<DocLibDocument>(doc.Id, new PartitionKey(doc.Id));

                var source = _bcc.GetBlobClient(storagePath);
                await source.DeleteAsync();
            }
            
        }

        public async Task<DocLibDocument> UpdateDocumentAsync(DocLibDocument doc)
        {
            doc.Validate();

            var db = _cosmosClient.GetDatabase(TableNames.Doclib);
            var docLibContainer = db.GetContainer(TableNames.Doclib);

            // Create category if not exists
            await CreateCatergoryAsync(doc, docLibContainer);

            // Create tags if not exists
            await CreateTagsAsync(doc, docLibContainer);

            doc.Tags = doc.Tags.Select(x => x.ToLower()).ToArray();
            doc.LastUpdate = DateTimeOffset.Now;

            await docLibContainer.UpsertItemAsync(doc, new PartitionKey(doc.Id));
            return doc;
        }
        public async Task<DocLibDocument> CreateDocumentAsync(DocLibDocument doc)
        {
            doc.Validate();

            var db = _cosmosClient.GetDatabase(TableNames.Doclib);
            var docLibContainer = db.GetContainer(TableNames.Doclib);

            // Create category if not exists
            await CreateCatergoryAsync(doc, docLibContainer);

            // Create tags if not exists
            await CreateTagsAsync(doc, docLibContainer);

            doc.Tags = doc.Tags.Select(x => x.ToLower()).ToArray();

            if (doc.Unsorted)
            {
                if (!doc.DigitalOnly)
                {
                    // Get folder or create a new one
                    var folder = GetCurrentFolder(docLibContainer) ?? await CreateFolderAsync(docLibContainer);

                    var register = folder.AddDocument();
                    doc.FolderName = folder.Name;
                    doc.RegisterName = register;
                    await docLibContainer.UpsertItemAsync(folder, new PartitionKey(folder.Id));
                }
                await MoveDocument(doc, docLibContainer);
                doc.Unsorted = false;
            }

            var guid = Guid.NewGuid();
            var id = $"Document.{doc.Name}.{guid}";
            doc.Id = id;
            doc.Name = $"{doc.Name}_{guid}";
            doc.LastUpdate = DateTimeOffset.Now;

            await docLibContainer.UpsertItemAsync(doc, new PartitionKey(id));
            return doc;
        }

        public async Task<bool> MoveDocumentAsync(DocLibDocument doc)
        {
            var db = _cosmosClient.GetDatabase(TableNames.Doclib);
            var docLibContainer = db.GetContainer(TableNames.Doclib);

            var dbDoc = docLibContainer.GetItemLinqQueryable<DocLibDocument>(true)
                .Where(x => x.Id == doc.Id)
                .AsEnumerable()
                .FirstOrDefault(x => x.Id == doc.Id);

            if (dbDoc == null || doc.FolderName == dbDoc.FolderName) return false;

            var oldFolder = docLibContainer.GetItemLinqQueryable<DocLibFolder>(true)
                .Where(x => x.Name == dbDoc.FolderName)
                .AsEnumerable()
                .FirstOrDefault();

            var folder = docLibContainer.GetItemLinqQueryable<DocLibFolder>(true)
                .Where(x => x.Name == doc.FolderName)
                .AsEnumerable()
                .FirstOrDefault();

            if (oldFolder == null || folder == null) return false;

            oldFolder.RemoveDocument(dbDoc.RegisterName);
            var register = folder.AddDocument();
            dbDoc.RegisterName = register;

            await docLibContainer.ReplaceItemAsync(folder, folder.Id, new PartitionKey(folder.Id));
            await docLibContainer.ReplaceItemAsync(oldFolder, oldFolder.Id, new PartitionKey(oldFolder.Id));
            await docLibContainer.ReplaceItemAsync(dbDoc, dbDoc.Id, new PartitionKey(dbDoc.Id));
            return true;
        }

        private async Task CreateCatergoryAsync(DocLibDocument doc, Container doclibContainer)
        {
            var id = $"Category.{doc.Category}";
            var category = doclibContainer.GetItemLinqQueryable<DocLibCategory>(true)
                .Where(x => x.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
            if (category == null)
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
        
        private async Task CreateTagsAsync(DocLibDocument doc, Container doclibContainer)
        {
            foreach (var docTag in doc.Tags)
            {
                var id = $"Tag.{docTag}";
                var tag = doclibContainer.GetItemLinqQueryable<DocLibTag>(true)
                    .Where(x => x.Id == id)
                    .AsEnumerable()
                    .FirstOrDefault();
                if (tag == null)
                {
                    var lowercased = docTag.ToLower();
                    await doclibContainer.CreateItemAsync(new DocLibTag
                    {
                        Id = id,
                        Name = lowercased,
                    });
                }
            }
        }

        private DocLibFolder GetCurrentFolder(Container doclibContainer)
        {
            return doclibContainer.GetItemLinqQueryable<DocLibFolder>(true)
                .Where(x => x.IsFull == false)
                .AsEnumerable()
                .FirstOrDefault();
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
            await doclibContainer.CreateItemAsync(newFolder, new PartitionKey(id));
            return newFolder;
        }

        private async Task MoveDocument(DocLibDocument doc, Container doclibContainer)
        {
            var unsortedEntry = doclibContainer.GetItemLinqQueryable<DocLibDocument>(true)
                                    .Where(x => x.Id == doc.Id)
                                    .AsEnumerable()
                                    .FirstOrDefault();
            if (unsortedEntry == null)
            {
                return;
            }
            await doclibContainer.DeleteItemAsync<DocLibDocument>(unsortedEntry.Id, new PartitionKey(unsortedEntry.Id));

            string newBlobLocation;
            if (doc.DigitalOnly)
            {
                doc.PhysicalName = doc.PhysicalName.Replace("unsorted/", "");
                newBlobLocation = $"digital/{doc.PhysicalName}";
            }
            else
            {
                doc.PhysicalName = doc.Unsorted ? doc.PhysicalName.Replace("unsorted/", "") : doc.PhysicalName;
                newBlobLocation = $"{doc.FolderName}/{doc.RegisterName}/{doc.PhysicalName}";
            }
            
            
            await MoveBlob(doc.BlobLocation, newBlobLocation);
            doc.BlobLocation = newBlobLocation;
        }

        private async Task MoveBlob(string from, string to)
        {
            var source = _bcc.GetBlobClient(from);
            if (await source.ExistsAsync())
            {
                var destBlob = _bcc.GetBlobClient(to);
                await destBlob.StartCopyFromUriAsync(source.Uri);
            }

            await source.DeleteAsync();
        }
    }
}
