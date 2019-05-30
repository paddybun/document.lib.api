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

            var response = documents.Select(doc => new GetDocumentResponse
            {
                Id = doc.Id.ToString(),
                Category = doc.Category.Name,
                Tags = doc.Tags.Select(t => t.Tag.Name).ToArray(),
                Name = doc.Name
            });
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult> PutDocument([FromForm]PutDocumentRequest request)
        {
            if (request.File?.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await request.File.CopyToAsync(stream);
                    await _documentService.UploadDocumentAsync(request.File.FileName, stream.ToArray());
                }
            }

            var document = await _documentService.SaveDocumentAsync(request.Name, request.Category, request.Folder, request.Tags);
            var tags = document.Tags?.Select(x => x.Tag.Name) ?? new string[0];

            return Ok(new PutDocumentResponse
            {
                Folder = document.Register?.Folder?.Name,
                Category = document.Category?.Name,
                Id = document.Id.ToString(),
                Tags = tags.ToArray(),
                Name = document.Name
            });
        }
    }
}