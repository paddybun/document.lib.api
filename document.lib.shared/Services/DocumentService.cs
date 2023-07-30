using Azure.Storage.Blobs;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IFolderService _folderService;
        private readonly BlobContainerClient _blobContainerClient;

        public DocumentService(BlobContainerClient blobContainerClient, IDocumentRepository documentRepository, ICategoryService categoryService, ITagService tagService, IFolderService folderService)
        {
            _documentRepository = documentRepository;
            _categoryService = categoryService;
            _tagService = tagService;
            _folderService = folderService;
            _blobContainerClient = blobContainerClient;
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
            if (folder != null) { folder = await _folderService.CreateNewFolderAsync(); }
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
