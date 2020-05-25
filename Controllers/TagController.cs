using System;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Models;
using document.lib.api.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace document.lib.api.Controllers
{
    [Route("api/[controller]")]
    public partial class TagController : Controller
    {
        private readonly DocumentlibContext _dbContext;
        private readonly TagService _tagService;

        public TagController(DocumentlibContext dbContext, TagService tagService)
        {
            _dbContext = dbContext;
            _tagService = tagService;
        }

        [HttpGet]
        public ActionResult GetTags()
        {
            var tags = _dbContext.Tags.OrderBy(tag => tag.Name);
            return Ok(tags);
        }

        [HttpGet("{id}")]
        public ActionResult GetTagById(Guid id)
        {
            var tag = _dbContext.Tags.Single(t => t.Id == id);
            return Ok(tag);
        }

        [HttpPut]
        public async Task<ActionResult> CreateTag([FromBody]PutTagRequest request)
        {
            var tag = new Tag { Name = request.Name };
            await _dbContext.Tags.AddAsync(tag);
            await _dbContext.SaveChangesAsync();
            return Ok(tag);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTag([FromBody]PostTagRequest request)
        {
            var tag = _dbContext.Tags.Single(t => t.Id == request.Id);
            tag.Name = request.Name;
            _dbContext.Tags.Update(tag);
            await _dbContext.SaveChangesAsync();
            return Ok(tag);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTag(Guid id)
        {
            await _tagService.DeleteTag(id);
            return Ok("deleted");
        }
    }

    public partial class TagController
    {
        public class PostTagRequest : PutTagRequest
        {
            [JsonProperty(PropertyName = "id")]
            public Guid Id { get; set; }
        }

        public class PutTagRequest
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
        }
    }
}