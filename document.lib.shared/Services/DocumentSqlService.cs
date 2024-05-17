using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;
using document.lib.shared.Models.Update;

namespace document.lib.shared.Services
{
    public class DocumentSqlService(
        BlobContainerClient blobContainerClient,
        IFolderRepository<EfFolder> folderRepository,
        IDocumentRepository<EfDocument> documentRepository,
        ICategoryRepository<EfCategory> categoryRepository,
        ITagRepository<EfTag> tagRepository)
        : IDocumentService
    {
        const string NewDocumentCategory = "uncategorized";
        const string NewDocumentRegister = "unsorted";

        public async Task<ITypedServiceResult<DocumentModel>> GetDocumentAsync(int id)
        {
            var doc = await documentRepository.GetDocumentAsync(id);
            return doc != null 
                ? ServiceResult.Ok(Map(doc)) 
                : ServiceResult.DefaultError<DocumentModel>();
        }

        public async Task<ITypedServiceResult<DocumentModel>> GetDocumentAsync(string name)
        {
            var doc = await documentRepository.GetDocumentAsync(name);
            return doc != null 
                ? ServiceResult.Ok(Map(doc)) 
                : ServiceResult.DefaultError<DocumentModel>();
        }

        public async Task<ITypedServiceResult<PagedResult<DocumentModel>>> GetUnsortedDocuments(int page, int pageSize)
        {
            var pagedResult = await documentRepository.GetUnsortedDocumentsAsync(page, pageSize);
            var documents = pagedResult.Results.Select(Map).ToList();

            var toReturn = new PagedResult<DocumentModel>(documents, pagedResult.Total);
            return ServiceResult.Ok(toReturn);
        }

        public async Task<ITypedServiceResult<PagedResult<DocumentModel>>> GetDocumentsForFolder(string folderName, int page, int pageSize)
        {
            var pagedResult = await documentRepository.GetDocumentsForFolderAsync(folderName, page, pageSize);
            var documents = pagedResult.Results.Select(Map).ToList();
            var toReturn = new PagedResult<DocumentModel>(documents, pagedResult.Total);
            return ServiceResult.Ok(toReturn);
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

            // move blob
            var newBlobLocation = $"{folderTo.Name}/{folderTo.CurrentRegister!.Name}/{document.PhysicalName}";
            await MoveBlob(document.BlobLocation, newBlobLocation);
            document.BlobLocation = newBlobLocation;
            await documentRepository.SaveAsync();
        }

        public async Task<ITypedServiceResult<PagedResult<DocumentModel>>> GetDocumentsPagedAsync(int page, int pageSize)
        {
            var pagedResult = await documentRepository.GetDocumentsPagedAsync(page, pageSize);
            var documents = pagedResult.Results.Select(Map).ToList();
            var toReturn = new PagedResult<DocumentModel>(documents, pagedResult.Total);
            return ServiceResult.Ok(toReturn);
        }

        public async Task<DocumentModel?> ModifyTagsAsync(int id, string[] toAdd, string[] toRemove)
        {
            var document = await documentRepository.GetDocumentAsync(id);
            if (document == null) return null;

            var availableTags = document.Tags.Select(x => x.Tag.Name).ToList();
            var tagsToAdd = toAdd.Where(x => !availableTags.Contains(x)).ToList();
            foreach (var add in tagsToAdd)
            {
                // create tag if it doesn't exist
                var tag = await tagRepository.GetTagByNameAsync(add) ?? await tagRepository.CreateTagAsync(add, add);
                document.Tags.Add(new EfTagAssignment { Document = document, Tag = tag });
            }

            var tagsToRemove = document.Tags.Where(ta => toRemove.Contains(ta.Tag.Name)).ToList();
            foreach (var remove in tagsToRemove)
            {
                document.Tags.Remove(remove);                
            }

            await documentRepository.SaveAsync();
            return Map(document);
        }

        public async Task<DocumentModel?> UpdateDocumentAsync(DocumentModel doc)
        {
            var documentId = (int)doc.Id!;
            var updateModel = DocumentUpdateModel.CreateFromModel(doc);
            var document = await documentRepository.GetDocumentAsync(documentId);
            if (document == null) return null;

            if (updateModel.Category != document.Category.Name)
            {
                var category = await categoryRepository.GetCategoryByNameAsync(updateModel.Category!);
                document.Category = category ?? document.Category;
            }
            document.DisplayName = updateModel.DisplayName;
            document.Company = updateModel.Company;
            document.DateOfDocument = updateModel.DateOfDocument;
            document.Description = updateModel.Description;

            await documentRepository.SaveAsync();
            return Map(document);
        }

        public async Task<DocumentModel> AddDocumentToIndexAsync(string blobPath)
        {
            var filename = blobPath.Split('/')[^1];
            var name = Guid.NewGuid().ToString();
            var folder = await folderRepository.GetFolderAsync(NewDocumentRegister);
            if (folder == null) throw new Exception("Unsorted folder not found. Please make sure a folder with the name \"unsorted\" exists");

            var category = await categoryRepository.GetCategoryByNameAsync(NewDocumentCategory);
            if (folder == null) throw new Exception("Uncategorized category not found. Please make sure a folder with the name \"uncategorized\" exists");
            
            var doc = new EfDocument
            {
                Name = name,
                PhysicalName = filename,
                BlobLocation = blobPath,
                UploadDate = DateTimeOffset.Now,
                Register = folder.CurrentRegister!,
                Unsorted = true,
                Category = category!
            };
            
            var document = await documentRepository.CreateDocumentAsync(doc);
            return Map(document);
        }

        public async Task<ITypedServiceResult<DocumentModel>> CreateDocumentAsync(DocumentModel model)
        {
            try
            {
                var doc = await documentRepository.GetDocumentAsync((int)model.Id!);
                var category = await categoryRepository.GetCategoryByNameAsync(model.CategoryName);
                var folder = (await folderRepository.GetActiveFoldersAsync()).FirstOrDefault();
                if (doc == null || doc.Unsorted == false || category == null || folder == null)
                {
                    return ServiceResult.DefaultError<DocumentModel>();
                }

                doc.Category = category;
                doc.DisplayName = model.DisplayName;
                doc.Company = model.Company;
                doc.DateOfDocument = model.DateOfDocument;
                doc.Description = model.Description;
                doc.Unsorted = false;

                AddDocumentToFolder(doc, folder);
                await documentRepository.SaveAsync();
                return ServiceResult.Ok(Map(doc));
            }
            catch
            {
                return ServiceResult.DefaultError<DocumentModel>();
            }
            
        }

        public Task DeleteDocumentAsync(DocumentModel doc)
        {
            throw new NotImplementedException();
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
