using System;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var folders = _documentlibContext.Folders
                .Include(f => f.Registers)
                .OrderBy(folder => folder.Name);

            var folderResponse = folders.Select(folder => new GetFolderResponse
            {
                Id = folder.Id.ToString(),
                Name = folder.Name,
                Registers = folder.Registers.Select(reg => reg.Id.ToString()).ToArray(),
                DocumentCount = folder.Registers.Sum(reg => reg.DocumentCount)
            });

            return Ok(folderResponse);
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
        public class GetFolderResponse
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string[] Registers { get; set; }
            public int DocumentCount { get; set; }
        }

        public class PutFolderRequest
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}