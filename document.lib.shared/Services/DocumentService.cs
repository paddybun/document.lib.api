using Azure.Storage.Blobs;
using document.lib.shared.Enums;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;
using Microsoft.Extensions.Options;

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
        public async Task<List<DocumentModel>> GetUnsortedDocuments()
        {
            return await documentRepository.GetUnsortedDocumentsAsync();
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
                return documentRepository.GetDocumentAsync(new DocumentQueryParameters(parsedId, name));
            }
            return documentRepository.GetDocumentAsync(new DocumentQueryParameters(name: id));
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
            var document = await documentRepository.CreateDocumentAsync(doc);
            var folder = await folderService.GetOrCreateActiveFolderAsync();
            if (folder != null) { folder = await folderService.SaveAsync(folder); }
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


            var dbDoc = await documentRepository.GetDocumentAsync(queryParams);
            if (dbDoc == null || doc.FolderName == dbDoc.FolderName) return false;

            var oldFolder = await folderService.GetFolderByNameAsync(dbDoc.FolderName);
            var newFolder = await folderService.GetFolderByNameAsync(doc.FolderName);

            if (oldFolder == null || newFolder == null) return false;
            await folderService.RemoveDocumentFromFolder(oldFolder, dbDoc);
            await folderService.AddDocumentToFolderAsync(newFolder, dbDoc);

            var relocatedDoc = await documentRepository.GetDocumentAsync(queryParams);
            dbDoc.BlobLocation = $"{newFolder.Name}/{relocatedDoc.RegisterName}/{dbDoc.PhysicalName}";
            
            await MoveBlob(oldPath, dbDoc.BlobLocation);
            await documentRepository.UpdateDocumentAsync(dbDoc);

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

            await documentRepository.DeleteDocumentAsync(doc);
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
