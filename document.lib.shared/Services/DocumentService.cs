using Azure.Storage.Blobs;
using document.lib.shared.Constants;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IFolderService _folderService;
        private readonly BlobContainerClient _bcc;
        private readonly CosmosClient _cosmosClient;

        public DocumentService(IOptions<AppConfiguration> config, IDocumentRepository documentRepository, ICategoryService categoryService, ITagService tagService, IFolderService folderService)
        {
            _documentRepository = documentRepository;
            _categoryService = categoryService;
            _tagService = tagService;
            _folderService = folderService;
            _bcc = new BlobContainerClient(config.Value.BlobContainerConnectionString, config.Value.BlobContainer);
            _cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        }

        public async Task DeleteDocumentAsync(DocLibDocument doc)
        {
            if (string.IsNullOrEmpty(doc.Id))
            {
                return;
            }

            await _documentRepository.DeleteDocumentAsync(doc);
        }

        public async Task<DocLibDocument> UpdateDocumentAsync(DocLibDocument doc)
        {
            doc.Validate();

            var db = _cosmosClient.GetDatabase(TableNames.Doclib);
            var docLibContainer = db.GetContainer(TableNames.Doclib);

            // Create category if not exists
            await _categoryService.CreateOrGetCategoryAsync(doc.Category);

            // Create tags if not exists
            await foreach (var tag in _tagService.CreateOrGetTagsAsync(doc.Tags)) { }

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
            await _categoryService.CreateOrGetCategoryAsync(doc.Category);

            // Create tags if not exists
            await foreach (var tag in _tagService.CreateOrGetTagsAsync(doc.Tags)) { }

            doc.Tags = doc.Tags.Select(x => x.ToLower()).ToArray();

            if (doc.Unsorted)
            {
                if (!doc.DigitalOnly)
                {
                    // Get folder or create a new one
                    var folder = _folderService.GetActiveFolder() ?? await CreateFolderAsync(docLibContainer);

                    var register = folder.AddDocument();
                    doc.FolderId = folder.Id;
                    doc.FolderName = folder.DisplayName;
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
