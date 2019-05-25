using System.IO;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Models;
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

            var category = _documentlibContext.Categories.SingleOrDefault(cat => cat.Id == request.Category);
            var folder = _documentlibContext.Folders.SingleOrDefault(f => f.Id == request.Folder);
            var reg = _documentlibContext.Registers.First();

            var newDoc = new LibDocument
            {
                Category = category,
                Name = request.Name,
                Register = reg
            };

            var tags = _documentlibContext.Tags.Where(tag => request.Tags.Contains(tag.Id));
            if (tags.Any())
            {
                var tagRelations = tags.Select(tag => new DocumentTag { Tag = tag, LibDocument = newDoc }).ToArray();
                newDoc.Tags = tagRelations;
            }

            await _documentlibContext.LibDocuments.AddAsync(newDoc);
            await _documentlibContext.SaveChangesAsync();
            return Ok(new PutDocumentResponse
            {
                Folder = folder?.Name,
                Category = category?.Name,
                Id = newDoc.Id.ToString(),
                Tags = tags.Select(t => t.Name).ToArray(),
                Name = newDoc.Name
            });
        }
    }
}