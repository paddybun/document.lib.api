using System;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace document.lib.api.Controllers
{
    [Route("api/[controller]")]
    public partial class CategoryController : Controller
    {
        private readonly DocumentlibContext _documentlibContext;

        public CategoryController(DocumentlibContext documentlibContext)
        {
            _documentlibContext = documentlibContext;
        }

        [HttpGet]
        public ActionResult GetCategories()
        {
            var categories = _documentlibContext.Categories.OrderBy(cat => cat.Id);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public ActionResult GetCategories(Guid id)
        {
            var categories = _documentlibContext.Categories.Single(cat => cat.Id == id);
            return Ok(categories);
        }

        [HttpPut]
        public async Task<ActionResult> PutCategory([FromBody]PutCategoryRequest request)
        {
            var newCategory = new Category
            {
                Name = request.Name,
                Abbreviation = request.Abbreviation
            };
            await _documentlibContext.AddAsync(newCategory);
            await _documentlibContext.SaveChangesAsync();
            return Ok(newCategory);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCategory([FromBody]PostCategoryRequest request)
        {
            var category = _documentlibContext.Categories.Single(cat => cat.Id == request.Id);
            category.Name = request.Name;
            category.Abbreviation = request.Abbreviation;

            _documentlibContext.Update(category);
            await _documentlibContext.SaveChangesAsync();
            return Ok(category);
        }
    }

    public partial class CategoryController
    {
        public class PostCategoryRequest : PutCategoryRequest
        {
            [JsonProperty("id")]
            public Guid Id { get; set; }
        }

        public class PutCategoryRequest
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("abbreviation")]
            public string Abbreviation { get; set; }
        }
    }
}