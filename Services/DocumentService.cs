using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Models;
using document.lib.api.Options;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace document.lib.api.Services
{
    public class DocumentService : IDocumentService
    {
        private CloudStorageAccount _storageAccount = null;
        private CloudBlobClient _blobClient = null;
        private CloudBlobContainer _cloudBlobContainer = null;
        private readonly IOptions<LibConnectionOptions> _options;
        private readonly DocumentlibContext _documentlibContext;

        public DocumentService(IOptions<LibConnectionOptions> options, DocumentlibContext documentlibContext)
        {
            _options = options;
            _documentlibContext = documentlibContext;
            Initialize();
        }

        protected void Initialize()
        {
            if (CloudStorageAccount.TryParse(_options.Value.AzureStorageConnection, out _storageAccount))
            {
                _blobClient = _storageAccount.CreateCloudBlobClient();
                _cloudBlobContainer = _blobClient.GetContainerReference("library-storage");

                if (!_cloudBlobContainer.Exists())
                {
                    _cloudBlobContainer.Create(BlobContainerPublicAccessType.Off);
                }
            }
        }

        public async Task UploadDocumentAsync(string blobname, byte[] buffer)
        {
            var blob = _cloudBlobContainer.GetBlockBlobReference(blobname);
            using (MemoryStream srcStream = new MemoryStream(buffer))
            {
                await blob.UploadFromStreamAsync(srcStream, null, null, null);
            }
        }

        public async Task<LibDocument> SaveDocumentAsync(string name, Guid categoryId, Guid folderId, Guid[] tagIds)
        {
            var category = _documentlibContext.Categories.SingleOrDefault(cat => cat.Id == categoryId);
            var folder = _documentlibContext.Folders
                .Include(f => f.Registers)
                .Single(f => f.Id == folderId);

            var register = folder.Registers?.SingleOrDefault(reg => reg.DocumentCount < 10) ?? new Register { DocumentCount = 0, Name = "A" };
            register.DocumentCount++;
            register.Folder = folder;

            var newDoc = new LibDocument
            {
                Category = category,
                Name = name,
                Register = register
            };

            var tags = _documentlibContext.Tags.Where(tag => tagIds.Contains(tag.Id));
            if (tags.Any())
            {
                var tagRelations = tags.Select(tag => new DocumentTag { Tag = tag, LibDocument = newDoc }).ToArray();
                newDoc.Tags = tagRelations;
            }

            await _documentlibContext.LibDocuments.AddAsync(newDoc);
            await _documentlibContext.SaveChangesAsync();
            return newDoc;
        }
    }
}