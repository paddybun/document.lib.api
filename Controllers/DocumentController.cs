using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace document.lib.api.Controllers
{
    [Route("api/[controller]")]
    public partial class DocumentController : Controller
    {
        private readonly DocumentlibContext _documentlibContext;
        private readonly IDocumentService _documentService;

        public DocumentController(DocumentlibContext documentlibContext, IDocumentService documentService)
        {
            _documentlibContext = documentlibContext;
            _documentService = documentService;
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
                Date = document.Date
            });
        }

        [HttpPost]
        public async Task<ActionResult> PostDocument([FromForm]PostDocumentRequest request)
        {
            var document = await _documentService.CreateDocumentAsync(request.Name, request.Category, request.Folder, request.Date, request.Tags);

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
                Date = document.Date
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
                Date = document.Date
            });
        }
    }
}