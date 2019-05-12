using System;
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

        [HttpPost]
        public async Task<ActionResult> CreateTag([FromBody]CreateTagRequest request)
        {
            var tag = new Tag { CreatedAt = DateTimeOffset.Now, Name = request.Name };
            _dbContext.Tags.Add(tag);
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