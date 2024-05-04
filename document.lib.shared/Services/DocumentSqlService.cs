using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;

namespace document.lib.shared.Services
{
    public class DocumentSqlService(
        BlobContainerClient blobContainerClient,
        IFolderRepository<EfFolder> folderRepository,
        IDocumentRepository<EfDocument> documentRepository,
        ICategoryService categoryService,
        ITagService tagService,
        IFolderService folderService)
        : IDocumentService
    {
        const string NewDocumentCategory = "uncategorized";
        const string NewDocumentRegister = "unsorted";

        public async Task<DocumentModel?> GetDocumentByIdAsync(int id)
        {
            var doc = await documentRepository.GetDocumentAsync(id);
            return doc != null ? Map(doc) : null;
        }

        public async Task<DocumentModel?> GetDocumentByNameAsync(string name)
        {
            var doc = await documentRepository.GetDocumentAsync(name);
            return doc != null ? Map(doc) : null;
        }

        public async Task<(int, List<DocumentModel>)> GetUnsortedDocuments(int page, int pageSize)
        {
            var (count, efDocuments) = await documentRepository.GetUnsortedDocumentsAsync(page, pageSize);
            var documents = efDocuments.Select(Map).ToList();
            return (count, documents);
        }

        public async Task MoveDocumentAsync(int documentId, int folderFromId, int toFolderId)
        {
            var document = await documentRepository.GetDocumentAsync(documentId);
            var folderFrom = await folderRepository.GetFolderAsync(folderFromId);
            var folderTo = await folderRepository.GetFolderAsync(toFolderId);
            if (document == null || folderTo == null || folderFrom == null) return;
        
            // remove document from old folder
            var registerFrom = folderFrom.Registers.Single(reg => reg.Documents.Any(doc => doc.Id == documentId));
            registerFrom.DocumentCount--;
            folderFrom.TotalDocuments--;
            
            // add document to new folder
            AddDocumentToFolder(document, folderTo);
            await documentRepository.SaveAsync();

            // move blob
            var newBlobLocation = $"{folderTo.Name}/{folderTo.CurrentRegister!.Name}/{document.PhysicalName}";
            await MoveBlob(document.BlobLocation, newBlobLocation);
            document.BlobLocation = newBlobLocation;
            await documentRepository.SaveAsync();
            
            await documentRepository.SaveAsync();
        }

        public async Task<(int, List<DocumentModel>)> GetDocumentsPagedAsync(int page, int pageSize)
        {
            var (count, efDocuments) = await documentRepository.GetDocumentsPagedAsync(page, pageSize);
            var documents = efDocuments.Select(Map).ToList();
            return (count, documents);
        }

        public async Task<DocumentModel> UpdateDocumentAsync(DocumentModel doc)
        {
            if (!IsValid(doc)) { throw new Exception("Please make sure the following fields are filled ['DisplayName, Company, DateOfDocument, Category, Tags']"); }

            var category = await categoryService.CreateOrGetCategoryAsync(doc.CategoryName);
            var tags = await tagService.GetOrCreateTagsAsync(doc.Tags ?? []);
            var folder = await folderService.GetFolderByNameAsync(doc.FolderId ?? "") ?? await folderService.CreateNewFolderAsync();

            if (doc.Unsorted)
            {
                await MoveDocumentFromUnsorted(doc);
            }

            var res = await documentRepository.UpdateDocumentAsync(doc, (int?)category.Id, folder, tags.ToArray());
            return Map(res);
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
            return Map(document);
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

                var props = (await destBlob.GetPropertiesAsync()).Value;
                while (props.CopyStatus == CopyStatus.Pending)
                {
                    await Task.Delay(50);
                    props = (await destBlob.GetPropertiesAsync()).Value;
                }
                
                if (props.CopyStatus == CopyStatus.Success)
                {
                    await source.DeleteAsync();
                }
            }
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
        
        private void AddDocumentToFolder(EfDocument doc, EfFolder folder)
        {
            var register = folder.CurrentRegister ?? CreateRegister(folder);
            register.Documents.Add(doc);
            register.DocumentCount++;
            folder.TotalDocuments++;
            folder.IsFull = folder.TotalDocuments >= folder.MaxDocumentsFolder;
        }
    
        private EfRegister CreateRegister(EfFolder folder)
        {
            var lastIx = int.Parse(folder.Registers.OrderByDescending(x => x.Name).First().Name);
            var newIx = (lastIx++).ToString();
            var register = new EfRegister
            {
                Name = newIx,
                Documents = [],
                DisplayName = newIx,
                DocumentCount = 0,
                Folder = folder
            };
            folder.Registers.Add(register);
            return register;
        }
        
        private DocumentModel Map(EfDocument efDocument)
        {
            var tags = efDocument.Tags?.Select(x => x.Tag?.Name ?? "").ToList() ?? [];

            return new DocumentModel
            {
                Id = efDocument.Id.ToString(),
                Name = efDocument.Name,
                DisplayName = efDocument.DisplayName,
                Description = efDocument.Description,
                CategoryName = efDocument.Category?.DisplayName ?? string.Empty,
                BlobLocation = efDocument.BlobLocation,
                FolderName = efDocument.Register?.Folder?.Name ?? string.Empty,
                Company = efDocument.Company,
                DateOfDocument = efDocument.DateOfDocument,
                DateModified = efDocument.DateModified,
                UploadDate = efDocument.DateCreated,
                Digital = efDocument.Digital,
                PhysicalName = efDocument.PhysicalName,
                Tags = tags,
                RegisterName = efDocument.Register?.Name ?? string.Empty,
                FolderId = efDocument.Register?.Folder?.Id.ToString() ?? string.Empty,
                Unsorted = efDocument.Unsorted
            };
        }

    }
}
