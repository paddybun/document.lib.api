using System.IO;
using System.Threading.Tasks;
using document.lib.api.Options;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;

namespace document.lib.api.Services
{
    public class DocumentService : IDocumentService
    {
        private CloudStorageAccount _storageAccount = null;
        private CloudBlobClient _blobClient = null;
        private CloudBlobContainer _cloudBlobContainer = null;
        private readonly IOptions<LibConnectionOptions> _options;

        public DocumentService(IOptions<LibConnectionOptions> options)
        {
            _options = options;
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
    }
}