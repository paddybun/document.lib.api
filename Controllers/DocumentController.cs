using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using document.lib.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace document.lib.api.Controllers
{
    [Route("api/[controller]")]
    public partial class DocumentController : Controller
    {
        private readonly DocumentlibContext _documentlibContext;

        public DocumentController(DocumentlibContext documentlibContext)
        {
            _documentlibContext = documentlibContext;
        }

        [HttpGet]
        public ActionResult GetDocuments()
        {
            var documents = _documentlibContext.LibDocuments
                .Include(doc => doc.Tags)
                    .ThenInclude(tag => tag.Tag)
                .Include(doc => doc.Folder)
                .Include(doc => doc.Category)
                .OrderBy(doc => doc.Id);

            var response = documents.Select(doc => new GetDocumentResponse
            {
                Id = doc.Id.ToString(),
                Folder = doc.Folder.Name,
                Category = doc.Category.Name,
                Tags = doc.Tags.Select(t => t.Tag.Name).ToArray(),
                Name = doc.Name
            });
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult> PutDocument([FromBody] PutDocumentRequest request)
        {
            var category = _documentlibContext.Categories.Single(cat => cat.Id == request.CategoryId);
            var folder = _documentlibContext.Folders.Single(f => f.Id == request.FolderId);

            var newDoc = new LibDocument
            {
                Category = category,
                Folder = folder,
                Name = request.Name
            };

            var tags = _documentlibContext.Tags.Where(tag => request.Tags.Contains(tag.Id));
            var tagRelations = tags.Select(tag => new DocumentTag { Tag = tag, LibDocument = newDoc }).ToArray();
            newDoc.Tags = tagRelations;

            await _documentlibContext.LibDocuments.AddAsync(newDoc);
            await _documentlibContext.SaveChangesAsync();
            return Ok(new PutDocumentResponse
            {
                Folder = folder.Name,
                Category = category.Name,
                Id = newDoc.Id.ToString(),
                Tags = tags.Select(t => t.Name).ToArray(),
                Name = newDoc.Name
            });
        }
    }

    public partial class DocumentController
    {
        public class PutDocumentResponse : GetDocumentResponse { }

        public class GetDocumentResponse
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string[] Tags { get; set; }
            public string Category { get; set; }
            public string Folder { get; set; }
        }

        public class PutDocumentRequest
        {
            [JsonProperty("folder")]
            public Guid FolderId { get; set; }

            [JsonProperty("category")]
            public Guid CategoryId { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public Guid[] Tags { get; set; }

        }
    }
}