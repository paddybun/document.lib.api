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
            var documents = _documentlibContext.LibDocuments.OrderBy(doc => doc.Id);
            var serialized = JsonConvert.SerializeObject(documents);
            return Ok(serialized);
        }

        [HttpPut]
        public async Task<ActionResult> PutDocument([FromBody] PutDocumentRequest request)
        {
            var tags = _documentlibContext.Tags.Where(tag => request.Tags.Contains(tag.Id));
            var tagRelations = tags.Select(tag => new DocumentTag {TagId = tag.Id}).ToArray();

            var newDoc = new LibDocument
            {
                Category = _documentlibContext.Categories.Single(cat => cat.Id == request.CategoryId),
                Folder = _documentlibContext.Folders.Single(folder => folder.Id == request.FolderId),
                Name = request.Name,
                Tags = tagRelations
            };

            await _documentlibContext.LibDocuments.AddAsync(newDoc);
            await _documentlibContext.SaveChangesAsync();
            return Ok(newDoc);
        }
    }

    public partial class DocumentController
    {
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