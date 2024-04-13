using Azure.Storage.Blobs;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;

namespace document.lib.shared.Services
{
    public class DocumentService(
        BlobContainerClient blobContainerClient,
        IDocumentRepository documentRepository,
        ICategoryService categoryService,
        ITagService tagService,
        IFolderService folderService)
        : IDocumentService
    {
        const string NewDocumentCategory = "uncategorized";
        const string NewDocumentRegister = "unsorted";

        public async Task<List<DocumentModel>> GetUnsortedDocuments()
        {
            return await documentRepository.GetUnsortedDocumentsAsync();
        }

        public async Task<DocumentModel?> GetDocumentAsync(string? id = null, string? name = null)
        {
            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(name))
            {
                var idNull = new ArgumentNullException(nameof(id));
                var nameNull = new ArgumentNullException(nameof(name));
                throw new AggregateException(idNull, nameNull);
            }

            DocumentModel model;
            if (int.TryParse(id, out var parsedId))
            {
                 model = new DocumentModel
                {
                    Id = parsedId.ToString()
                };
            }
            else if (!string.IsNullOrWhiteSpace(name))
            {
                model = new DocumentModel
                {
                    Name = name
                };
            }
            else
            {
                return null;
            }

            return await documentRepository.GetDocumentAsync(model);
        }

        public async Task<List<DocumentModel>> GetAllDocumentsAsync()
        {
            return await documentRepository.GetAllDocumentsAsync();
        }

        public async Task<DocumentModel> UpdateDocumentAsync(DocumentModel doc)
        {
            if (!IsValid(doc)) { throw new Exception("Please make sure the following fields are filled ['DisplayName, Company, DateOfDocument, Category, Tags']"); }

            var category = await categoryService.CreateOrGetCategoryAsync(doc.CategoryName);
            var tags = await tagService.GetOrCreateTagsAsync(doc.Tags);
            var folder = await folderService.GetOrCreateFolderByIdAsync(doc.FolderId);

            if (doc.Unsorted)
            {
                await MoveDocumentFromUnsorted(doc);
            }

            var res = await documentRepository.UpdateDocumentAsync(doc, category, folder, tags.ToArray());
            return res;
        }

        public async Task<DocumentModel> CreateNewDocumentAsync(DocumentModel doc)
        {
            if (!IsValid(doc)) { throw new Exception("Please make sure the following fields are filled ['DisplayName, Company, DateOfDocument, Category, Tags']"); }

            if (doc.Unsorted)
            {
                // overwrite what ever the user has set for a new document
                doc.CategoryName = NewDocumentCategory;
                doc.RegisterName = NewDocumentRegister;
            }

            var document = await documentRepository.CreateDocumentAsync(doc);
            var folder = await folderService.GetOrCreateActiveFolderAsync();
            if (folder != null) { folder = await folderService.SaveAsync(folder); }
            return document;
        }

        public async Task<bool> MoveDocumentAsync(DocumentModel doc)
        {
            var oldPath = doc.BlobLocation;
            
            var model = new DocumentModel { };

            if (int.TryParse(doc.Id, out var id))
                model.Id = id.ToString();
            else
                model.Name = doc.Id;

            var dbDoc = await documentRepository.GetDocumentAsync(model);
            if (dbDoc == null || doc.FolderName == dbDoc.FolderName) return false;

            var oldFolder = await folderService.GetFolderByNameAsync(dbDoc.FolderName);
            var newFolder = await folderService.GetFolderByNameAsync(doc.FolderName);

            if (oldFolder == null || newFolder == null) return false;
            await folderService.RemoveDocumentFromFolder(oldFolder, dbDoc);
            await folderService.AddDocumentToFolderAsync(newFolder, dbDoc);

            var relocatedDoc = await documentRepository.GetDocumentAsync(model);
            dbDoc.BlobLocation = $"{newFolder.Name}/{relocatedDoc.RegisterName}/{dbDoc.PhysicalName}";
            
            await MoveBlob(oldPath, dbDoc.BlobLocation);
            await documentRepository.UpdateDocumentAsync(dbDoc);

            return true;
        }

        public Task DeleteDocumentAsync(DocumentModel doc)
        {
            throw new NotImplementedException();
        }

        private async Task MoveDocumentFromUnsorted(DocumentModel doc)
        {
            var dbFolder = await folderService.GetFolderByNameAsync("unsorted");
            var newFolder = await folderService.GetOrCreateActiveFolderAsync();
            await folderService.RemoveDocumentFromFolder(dbFolder, doc);

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
            var source = blobContainerClient.GetBlobClient(from);
            if (await source.ExistsAsync())
            {
                var destBlob = blobContainerClient.GetBlobClient(to);
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
