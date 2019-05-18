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
            var result = _dbContext.Tags.OrderBy(tag => tag.Name);
            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpPost]
        public async Task<ActionResult> CreateTag([FromBody]CreateTagRequest request)
        {
            var tag = new Tag { Name = request.Name };
            _dbContext.Tags.Add(tag);
            await _dbContext.SaveChangesAsync();
            return Ok();
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