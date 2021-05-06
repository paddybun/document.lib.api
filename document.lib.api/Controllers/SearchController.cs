using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace document.lib.api.Controllers
{
    public partial class SearchController : Controller
    {
        public async Task<IActionResult> Search([FromBody] SearchRequest searchRequest)
        {
            return Ok("test");
        }
    }

    public partial class SearchController
    {
        public class SearchRequest
        {
            [JsonProperty("term")]
            public string Term { get; set; }

            [JsonProperty("from")]
            public DateTime? From { get; set; }

            [JsonProperty("to")]
            public DateTime? To { get; set; }
            
            [JsonProperty("tags")]
            public string[] Tags { get; set; }

            [JsonProperty("mustIncludeAll")]
            public bool MustIncludeAll { get; set; }

            [JsonProperty("page")]
            public int? Page { get; set; }

            [JsonProperty("pageSize")]
            public int? PageSize { get; set; }
        }
    }
}