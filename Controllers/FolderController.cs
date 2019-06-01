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

            var folderResponse = folders.Select(folder => new FolderResponse
            {
                Id = folder.Id.ToString(),
                Name = folder.Name,
                Registers = folder.Registers.Select(reg => reg.Id.ToString()).ToArray(),
                DocumentCount = folder.Registers.Sum(reg => reg.DocumentCount)
            });

            return Ok(folderResponse);
        }

        [HttpGet("{id}")]
        public ActionResult GetFolder(Guid id)
        {
            var folder = _documentlibContext.Folders
                .Include(f => f.Registers)
                .Single(f => f.Id == id);

            var folderResponse = new FolderResponse
            {
                Id = folder.Id.ToString(),
                Name = folder.Name,
                Registers = folder.Registers.Select(reg => reg.Id.ToString()).ToArray(),
                DocumentCount = folder.Registers.Sum(reg => reg.DocumentCount)
            };

            return Ok(folderResponse);
        }

        [HttpPost]
        public async Task<ActionResult> PostFolder([FromBody]PostFolderRequest request)
        {
            var newFolder = new Folder
            {
                Name = request.Name
            };

            newFolder.Registers.Add(new Register
            {
                Name = "A",
                DocumentCount = 0
            });

            await _documentlibContext.AddAsync(newFolder);
            await _documentlibContext.SaveChangesAsync();

            var folderResponse = new FolderResponse
            {
                Id = newFolder.Id.ToString(),
                Name = newFolder.Name,
                Registers = newFolder.Registers.Select(reg => reg.Id.ToString()).ToArray(),
                DocumentCount = newFolder.Registers.Sum(reg => reg.DocumentCount)
            };

            return Ok(folderResponse);
        }

        [HttpPut]
        public async Task<ActionResult> PutFolder([FromBody]PutFolderRequest request)
        {
            var folder = await _documentlibContext.Folders.FindAsync(request.Id);
            folder.Name = request.Name;

            _documentlibContext.Update(folder);
            await _documentlibContext.SaveChangesAsync();

            var response = new FolderResponse
            {
                Id = folder.Id.ToString(),
                Name = folder.Name,
                Registers = folder.Registers?.Select(reg => reg.Name).ToArray() ?? new string[0],
                DocumentCount = folder.Registers?.Sum(reg => reg.DocumentCount) ?? 0
            };

            return Ok(response);
        }
    }

    public partial class FolderController
    {
        public class FolderResponse
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string[] Registers { get; set; }
            public int DocumentCount { get; set; }
        }

        public class PostFolderRequest
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public class PutFolderRequest : PostFolderRequest
        {
            [JsonProperty("id")]
            public Guid Id { get; set; }
        }
    }
}