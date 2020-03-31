using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Options;
using document.lib.api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace document.lib.api.Controllers
{
    [Route("api/[controller]")]
    public partial class DocumentController : Controller
    {
        private readonly DocumentlibContext _documentlibContext;
        private readonly IDocumentService _documentService;
        private readonly IOptions<LibStorageOptions> _storageOptions;

        public DocumentController(DocumentlibContext documentlibContext, IDocumentService documentService, IOptions<LibStorageOptions> storageOptions)
        {
            _documentlibContext = documentlibContext;
            _documentService = documentService;
            _storageOptions = storageOptions;
        }

        [HttpGet]
        public ActionResult GetDocuments()
        {
            var documents = _documentlibContext.LibDocuments
                .Include(doc => doc.Tags)
                    .ThenInclude(tag => tag.Tag)
                .Include(doc => doc.Category)
                .OrderBy(doc => doc.Id);

            var response = documents.Select(doc => new DocumentReponse
            {
                Id = doc.Id.ToString(),
                Category = doc.Category.Name,
                Tags = doc.Tags.Select(t => t.Tag.Name).ToArray(),
                Name = doc.Name
            });
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetDocument(Guid id)
        {
            var document = await _documentlibContext.LibDocuments
                .Include(doc => doc.Tags)
                .Include(doc => doc.Register)
                .ThenInclude(reg => reg.Folder)
                .Include(doc => doc.Category)
                .Include(doc => doc.Tags)
                .ThenInclude(relTag => relTag.Tag)
                .SingleOrDefaultAsync(doc => doc.Id == id);

            if (document == null)
            {
                return NotFound();
            }

            var tags = document.Tags?.Select(x => x.Tag.Name) ?? new string[0];
            return Ok(new DocumentReponse
            {
                Folder = document.Register?.Folder?.Id.ToString(),
                Category = document.Category?.Id.ToString(),
                Id = document.Id.ToString(),
                Tags = tags.ToArray(),
                Name = document.Name,
                Date = document.Date,
                Blobname = document.Blobname,
                DownloadLink = $"{_storageOptions.Value.StorageAccount}/{_storageOptions.Value.StorageContainer}/{document.Blobname}{_storageOptions.Value.StorageSas}"
            });
        }

        [HttpPost]
        public async Task<ActionResult> PostDocument([FromForm]PostDocumentRequest request)
        {
            var blobname = request.File?.FileName;
            var document = await _documentService.CreateDocumentAsync(blobname, request);

            if (request.File?.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await request.File.CopyToAsync(stream);
                    await _documentService.UploadDocumentAsync(blobname, document.Register, stream.ToArray());
                }
            }

            var tags = document.Tags?.Select(x => x.Tag.Name) ?? new string[0];

            return Ok(new DocumentReponse
            {
                Folder = document.Register?.Folder?.Name,
                Category = document.Category?.Name,
                Id = document.Id.ToString(),
                Tags = tags.ToArray(),
                Name = document.Name,
                Date = document.Date,
                Blobname = document.Blobname
            });
        }

        [HttpPut]
        public async Task<ActionResult> PutDocument([FromForm]PutDocumentRequest request)
        {
            var document = await _documentService.UpdateDocumentAsync(request.Id, request.Name, request.Category, request.Folder, request.Date, request.Tags);

            if (request.File?.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await request.File.CopyToAsync(stream);
                    await _documentService.UploadDocumentAsync(request.File.FileName, document.Register, stream.ToArray());
                }
            }

            var tags = document.Tags?.Select(x => x.Tag.Name) ?? new string[0];

            return Ok(new DocumentReponse
            {
                Folder = document.Register?.Folder?.Name,
                Category = document.Category?.Name,
                Id = document.Id.ToString(),
                Tags = tags.ToArray(),
                Name = document.Name,
                Date = document.Date,
                Blobname = document.Blobname
            });
        }

        [HttpGet("download/{id}")]
        public async Task<ActionResult> DownloadDocument(Guid id)
        {
            try
            {
                var doc = await _documentService.DownloadDocumentAsync(id);
                return File(doc.Data, doc.ContentType, doc.Filename);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDocument(Guid id)
        {
            try
            {
                await _documentService.DeleteDocumentAsync(id);
                return Ok("deleted");
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }
    }
}