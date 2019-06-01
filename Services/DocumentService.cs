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

        public async Task UploadDocumentAsync(string filename, Register register, byte[] buffer)
        {
            var blobname = $"{register.Folder.Name}/{register.Name}/{filename}";

            var blob = _cloudBlobContainer.GetBlockBlobReference(blobname);
            using (MemoryStream srcStream = new MemoryStream(buffer))
            {
                await blob.UploadFromStreamAsync(srcStream, null, null, null);
            }
        }

        public async Task<LibDocument> CreateDocumentAsync(string name, Guid categoryId, Guid folderId, DateTimeOffset date, Guid[] tagIds)
        {
            var category = _documentlibContext.Categories.SingleOrDefault(cat => cat.Id == categoryId);
            var folder = _documentlibContext.Folders
                .Include(f => f.Registers)
                .Single(f => f.Id == folderId);

            var register = GetNextRegister(folder);
            register.DocumentCount++;

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

        private Register GetNextRegister(Folder folder)
        {
            Register registerToUse;
            var latestRegister = folder.Registers.OrderByDescending(reg => reg.Name).FirstOrDefault();

            if (latestRegister == null)
            {
                registerToUse = new Register
                {
                    DocumentCount = 0,
                    Folder = folder,
                    Name = "A"
                };
            }
            else if (latestRegister.DocumentCount >= 10 && !latestRegister.Name.Equals("Z"))
            {
                if (latestRegister.Name.Equals("Z"))
                {
                    // TODO: Inform user nicely, that a new Folder has to be created and used.
                    throw new Exception("Please start a new Folder.");
                }

                var newName = Convert.ToChar(latestRegister.Name) + 1;
                registerToUse = new Register
                {
                    DocumentCount = 0,
                    Name = ((char)newName).ToString(),
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