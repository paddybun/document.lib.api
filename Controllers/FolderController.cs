using System;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace document.lib.api.Controllers
{
    [Route("api/[controller]")]
    public partial class FolderController : Controller
    {
        private readonly DocumentlibContext _documentlibContext;

        public FolderController(DocumentlibContext documentlibContext)
        {
            _documentlibContext = documentlibContext;
        }

        [HttpGet]
        public ActionResult GetFolders()
        {
            var folders = _documentlibContext.Folders.OrderBy(folder => folder.Name);
            return Ok(folders);
        }

        [HttpPut]
        public async Task<ActionResult> PutFolders([FromBody]PutFolderRequest request)
        {
            var newFolder = new Folder
            {
                Name = request.Name
            };
            await _documentlibContext.AddAsync(newFolder);
            await _documentlibContext.SaveChangesAsync();
            return Ok(newFolder);
        }
    }

    public partial class FolderController
    {
        public class PutFolderRequest
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}