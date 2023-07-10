﻿using Azure.Storage.Blobs;
using document.lib.shared.Interfaces;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using DocLibDocument = document.lib.shared.TableEntities.DocLibDocument;

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
            var category = await _categoryService.CreateOrGetCategoryAsync(doc.Category);
            var tags = await _tagService.CreateOrGetTagsAsync(doc.Tags);
            var folder = await _folderService.CreateOrGetFolderByIdAsync(doc.FolderId);

            if (doc.Unsorted)
            {
                await MoveDocumentFromUnsorted(doc);
            }

            var res = await _documentRepository.UpdateDocumentAsync(doc, category, folder, tags.ToArray());
            return res;
        }

        public async Task<DocLibDocument> CreateNewDocumentAsync(DocLibDocument doc)
        {
            doc.Validate();
            var document = await _documentRepository.CreateDocumentAsync(doc);
            var folder = await _folderService.GetActiveFolderAsync();
            if (folder != null) { folder = await _folderService.CreateNewFolderAsync(); }
            return document;
        }

        public async Task<bool> MoveDocumentAsync(DocLibDocument doc)
        {
            var oldPath = doc.BlobLocation;
            var dbDoc = await _documentRepository.GetDocumentById(doc.Id);
            if (dbDoc == null || doc.FolderName == dbDoc.FolderName) return false;

            var oldFolder = await _folderService.GetFolderByNameAsync(dbDoc.FolderName);
            var newFolder = await _folderService.GetFolderByNameAsync(doc.FolderName);

            if (oldFolder == null || newFolder == null) return false;
            await _folderService.RemoveDocumentFromFolder(oldFolder, dbDoc);
            await _folderService.AddDocumentToFolderAsync(newFolder, dbDoc);

            var relocatedDoc = await _documentRepository.GetDocumentById(doc.Id);
            dbDoc.BlobLocation = $"{newFolder.Name}/{relocatedDoc.RegisterName}/{dbDoc.PhysicalName}";
            
            await MoveBlob(oldPath, dbDoc.BlobLocation);
            await _documentRepository.UpdateDocumentAsync(dbDoc);

            return true;
        }

        private async Task MoveDocumentFromUnsorted(DocLibDocument doc)
        {
            var dbFolder = await _folderService.GetFolderByNameAsync("unsorted");
            var newFolder = await _folderService.GetActiveFolderAsync();
            await _folderService.RemoveDocumentFromFolder(dbFolder, doc);

            string newBlobLocation;
            if (doc.DigitalOnly)
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
    }
}
