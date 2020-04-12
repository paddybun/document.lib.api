using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Controllers;
using document.lib.api.Dtos;
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

        public async Task UploadDocumentAsync(string filename, Register register, byte[] buffer)
        {
            var blobname = GetBlobname(register, filename);

            var blob = _cloudBlobContainer.GetBlockBlobReference(blobname);
            using (MemoryStream srcStream = new MemoryStream(buffer))
            {
                await blob.UploadFromStreamAsync(srcStream, null, null, null);
            }
        }

        public async Task<DocumentDownloadDto> DownloadDocumentAsync(Guid docId)
        {
            var document = await _documentlibContext.LibDocuments.SingleOrDefaultAsync(doc => doc.Id == docId);
            var blob = _cloudBlobContainer.GetBlockBlobReference(document.Blobname);
            using (var ms = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(ms);
                var blobStream = await blob.OpenReadAsync();
                var name = blob.Name.Split('/').Last();
                return new DocumentDownloadDto
                {
                    Data = blobStream,
                    ContentType = blob.Properties.ContentType,
                    Filename = name
                };
            }
        }

        public async Task DeleteDocumentAsync(Guid docId)
        {
            var document = await _documentlibContext.LibDocuments.SingleOrDefaultAsync(doc => doc.Id == docId);

            var blob = _cloudBlobContainer.GetBlockBlobReference(document.Blobname);
            await blob.DeleteAsync();
            
            _documentlibContext.LibDocuments.Remove(document);
            await _documentlibContext.SaveChangesAsync();
        }

        public async Task<LibDocument> CreateDocumentAsync(string blobname, DocumentController.PostDocumentRequest request)
        {
            var category = _documentlibContext.Categories.SingleOrDefault(cat => cat.Id == request.Category);
            var folder = _documentlibContext.Folders
                .Include(f => f.Registers)
                .Single(f => f.Id == request.Folder);

            var register = GetNextRegister(folder);
            register.DocumentCount++;

            var newDoc = new LibDocument
            {
                Category = category,
                Name = request.Name,
                Register = register,
                Date = request.Date,
                Blobname = GetBlobname(register, blobname)
            };

            var tags = _documentlibContext.Tags.Where(tag => request.Tags.Contains(tag.Id));
            if (tags.Any())
            {
                var tagRelations = tags.Select(tag => new DocumentTag { Tag = tag, LibDocument = newDoc }).ToArray();
                newDoc.Tags = tagRelations;
            }

            await _documentlibContext.LibDocuments.AddAsync(newDoc);
            await _documentlibContext.SaveChangesAsync();
            return newDoc;
        }

        public async Task<LibDocument> UpdateDocumentAsync(Guid docId, string name, Guid categoryId, Guid folderId, DateTimeOffset date, Guid[] tagIds)
        {
            var category = _documentlibContext.Categories.SingleOrDefault(cat => cat.Id == categoryId);

            var document = await _documentlibContext.LibDocuments
                .Include(doc => doc.Category)
                .Include(doc => doc.Register)
                .ThenInclude(reg => reg.Folder)
                .SingleOrDefaultAsync(doc => doc.Id == docId);

            document.Name = name;
            document.Category = category;

            if (document.Register.Folder.Id != folderId)
            {
                var folder = _documentlibContext.Folders
                    .Include(f => f.Registers)
                    .Single(f => f.Id == folderId);

                var register = GetNextRegister(folder);

                register.DocumentCount++;
                document.Register.DocumentCount--;
                _documentlibContext.Update(document.Register);
                document.Register = register;
            }

            // TODO: Implement tag change detection.
            var tags = _documentlibContext.Tags.Where(tag => tagIds.Contains(tag.Id));
            if (tags.Any())
            {
                var tagRelations = tags.Select(tag => new DocumentTag { Tag = tag, LibDocument = document }).ToArray();
                document.Tags = tagRelations;
            }

            _documentlibContext.Update(document);
            await _documentlibContext.SaveChangesAsync();
            return document;
        }

        private string GetBlobname(Register register, string filename)
        {
            return $"{register.Folder.Name}/{filename}";
        }

        private Register GetNextRegister(Folder folder)
        {
            Register registerToUse;
            var latestRegister = folder.Registers.OrderByDescending(reg => reg.Order).FirstOrDefault();

            if (latestRegister == null)
            {
                registerToUse = new Register
                {
                    DocumentCount = 0,
                    Order = 0,
                    Folder = folder
                };
            }
            else if (latestRegister.DocumentCount >= 10)
            {
                registerToUse = new Register
                {
                    DocumentCount = 0,
                    Order = folder.Registers.Max(reg => reg.Order) + 1,
                    Folder = folder
                };
            }
            else
            {
                registerToUse = latestRegister;
            }

            return registerToUse;
        }
    }
}