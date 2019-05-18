using System;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace document.lib.api.Controllers
{
    [Route("api/[controller]")]
    public partial class TagController : Controller
    {
        private readonly DocumentlibContext _dbContext;

        public TagController(DocumentlibContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult GetTags()
        {
            var tags = _dbContext.Tags.OrderBy(tag => tag.Name);
            return Ok(tags);
        }

        [HttpPut]
        public async Task<ActionResult> CreateTag([FromBody]CreateTagRequest request)
        {
            var tag = new Tag { Name = request.Name };
            await _dbContext.Tags.AddAsync(tag);
            await _dbContext.SaveChangesAsync();
            return Ok(tag);
        }
    }

    public partial class TagController
    {
        public class CreateTagRequest
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
        }
    }
}