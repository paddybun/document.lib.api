using Azure.Storage.Blobs;
using document.lib.shared.Enums;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IFolderService _folderService;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly AppConfiguration _appConfig;

        public DocumentService(IOptions<AppConfiguration> options, BlobContainerClient blobContainerClient, IDocumentRepository documentRepository, ICategoryService categoryService, ITagService tagService, IFolderService folderService)
        {
            _documentRepository = documentRepository;
            _categoryService = categoryService;
            _tagService = tagService;
            _folderService = folderService;
            _blobContainerClient = blobContainerClient;
            _appConfig = options.Value;
        }

        public async Task<List<DocumentModel>> GetUnsortedDocuments()
        {
            return await _documentRepository.GetUnsortedDocumentsAsync();
        }

        public Task<DocumentModel> GetDocumentAsync(string id = null, string name = null)
        {
            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(name))
            {
                var idNull = new ArgumentNullException(nameof(id));
                var nameNull = new ArgumentNullException(nameof(id));
                throw new AggregateException(idNull, nameNull);
            }

            if (int.TryParse(id, out var parsedId))
            {
                return _documentRepository.GetDocumentAsync(new DocumentQueryParameters(parsedId, name));
            }
            return _documentRepository.GetDocumentAsync(new DocumentQueryParameters(name: id));
        }

        public async Task<List<DocumentModel>> GetAllDocumentsAsync()
        {
            var docs = new List<DocumentModel>();
            switch (_appConfig.DatabaseProvider)
            {
                case DatabaseProvider.Sql:

                    var docsAvailable = true;
                    var lastId = 0;
                    while (docsAvailable)
                    {
                        var tmpDocs = await _documentRepository.GetDocumentsAsync(lastId, 20);
                        docsAvailable = tmpDocs.Any();
                        docs.AddRange(tmpDocs);
                        lastId = tmpDocs.Max(x => int.Parse(x.Id));
                    }
                    break;
                case DatabaseProvider.Cosmos:
                    // Cosmos repo ignores pagination
                    docs.AddRange(await _documentRepository.GetDocumentsAsync(0, 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return docs;
        }

        public async Task<DocumentModel> UpdateDocumentAsync(DocumentModel doc)
        {
            if (!IsValid(doc)) { throw new Exception("Please make sure the following fields are filled ['DisplayName, Company, DateOfDocument, Category, Tags']"); }

            var category = await _categoryService.CreateOrGetCategoryAsync(doc.CategoryName);
            var tags = await _tagService.GetOrCreateTagsAsync(doc.Tags);
            var folder = await _folderService.GetOrCreateFolderByIdAsync(doc.FolderId);

            if (doc.Unsorted)
            {
                await MoveDocumentFromUnsorted(doc);
            }

            var res = await _documentRepository.UpdateDocumentAsync(doc, category, folder, tags.ToArray());
            return res;
        }

        public async Task<DocumentModel> CreateNewDocumentAsync(DocumentModel doc)
        {
            if (!IsValid(doc)) { throw new Exception("Please make sure the following fields are filled ['DisplayName, Company, DateOfDocument, Category, Tags']"); }
            var document = await _documentRepository.CreateDocumentAsync(doc);
            var folder = await _folderService.GetOrCreateActiveFolderAsync();
            if (folder != null) { folder = await _folderService.SaveAsync(folder); }
            return document;
        }

        public async Task<bool> MoveDocumentAsync(DocumentModel doc)
        {
            var oldPath = doc.BlobLocation;
            var queryParams = new DocumentQueryParameters();
            
            if (int.TryParse(doc.Id, out var id))
                queryParams.Id = id;
            else
                queryParams.Name = doc.Id;


            var dbDoc = await _documentRepository.GetDocumentAsync(queryParams);
            if (dbDoc == null || doc.FolderName == dbDoc.FolderName) return false;

            var oldFolder = await _folderService.GetFolderByNameAsync(dbDoc.FolderName);
            var newFolder = await _folderService.GetFolderByNameAsync(doc.FolderName);

            if (oldFolder == null || newFolder == null) return false;
            await _folderService.RemoveDocumentFromFolder(oldFolder, dbDoc);
            await _folderService.AddDocumentToFolderAsync(newFolder, dbDoc);

            var relocatedDoc = await _documentRepository.GetDocumentAsync(queryParams);
            dbDoc.BlobLocation = $"{newFolder.Name}/{relocatedDoc.RegisterName}/{dbDoc.PhysicalName}";
            
            await MoveBlob(oldPath, dbDoc.BlobLocation);
            await _documentRepository.UpdateDocumentAsync(dbDoc);

            return true;
        }

        public async Task DeleteDocumentAsync(DocumentModel doc)
        {
            // HACK: Temporary disable delete functionality
            throw new NotImplementedException();
            if (string.IsNullOrEmpty(doc.Id))
            {
                return;
            }

            await _documentRepository.DeleteDocumentAsync(doc);
        }

        private async Task MoveDocumentFromUnsorted(DocumentModel doc)
        {
            var dbFolder = await _folderService.GetFolderByNameAsync("unsorted");
            var newFolder = await _folderService.GetOrCreateActiveFolderAsync();
            await _folderService.RemoveDocumentFromFolder(dbFolder, doc);

            string newBlobLocation;
            if (doc.Digital)
            {
                doc.PhysicalName = doc.PhysicalName.Replace("unsorted/", "");
                newBlobLocation = $"digital/{doc.PhysicalName}";
            }
            else
            {
                doc.PhysicalName = doc.Unsorted ? doc.PhysicalName.Replace("unsorted/", "") : doc.PhysicalName;
                newBlobLocation = $"{newFolder.Name}/{newFolder.CurrentRegister}/{doc.PhysicalName}";
            }
            
            
            await MoveBlob(doc.BlobLocation, newBlobLocation);
            doc.BlobLocation = newBlobLocation;
        }

        private async Task MoveBlob(string from, string to)
        {
            var source = _blobContainerClient.GetBlobClient(from);
            if (await source.ExistsAsync())
            {
                var destBlob = _blobContainerClient.GetBlobClient(to);
                await destBlob.StartCopyFromUriAsync(source.Uri);
            }

            await source.DeleteAsync();
        }

        private bool IsValid(DocumentModel model)
        {
            return !string.IsNullOrWhiteSpace(model.DisplayName) &&
                   !string.IsNullOrWhiteSpace(model.Company) &&
                   model.DateOfDocument != default &&
                   !string.IsNullOrWhiteSpace(model.CategoryName) &&
                   model.Tags != null &&
                   model.Tags.Any();
        }
    }
}
