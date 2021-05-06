using System;
using System.Linq;
using System.Threading.Tasks;
using document.lib.api.Models;
using document.lib.api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace document.lib.api.Controllers
{
    [Route("api/[controller]")]
    public partial class FolderController : Controller
    {
        private readonly DocumentlibContext _documentlibContext;
        private readonly IRegisterService _registerService;

        public FolderController(DocumentlibContext documentlibContext, IRegisterService registerService)
        {
            _documentlibContext = documentlibContext;
            _registerService = registerService;
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
                .ThenInclude(reg => reg.Documents)
                .Single(f => f.Id == id);

            var folderResponse = new FolderResponse
            {
                Id = folder.Id.ToString(),
                Name = folder.Name,
                Registers = folder.Registers.Select(reg => reg.Id.ToString()).ToArray(),
                DocumentCount = folder.Registers.Sum(reg => reg.Documents.Count)
            };

            return Ok(folderResponse);
        }

        [HttpGet("registers/{id}")]
        public async Task<ActionResult> GetRegisters(Guid id)
        {
            var folder = await _documentlibContext.Folders
                .Include(f => f.Registers)
                .ThenInclude(reg => reg.Documents)
                .SingleOrDefaultAsync(f => f.Id == id);

            var registers = folder.Registers.OrderBy(reg => reg.Order).Select(reg => new RegisterResponse
            {
                Id = reg.Id.ToString(),
                DocumentCount = reg.Documents.Count,
                DisplayName = reg.DisplayName,
                Order = reg.Order,
                IsActive = reg.IsActive
            });
            return Ok(registers);
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
                Order = 0,
                DocumentCount = 0,
                IsActive = true
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
                Registers = folder.Registers?.Select(reg => reg.Id.ToString()).ToArray() ?? new string[0],
                DocumentCount = folder.Registers?.Sum(reg => reg.DocumentCount) ?? 0
            };

            return Ok(response);
        }

        [HttpPut("register")]
        public async Task<ActionResult> PutRegister([FromBody]PutRegisterRequest request)
        {
            var register = await _registerService.UpdateRegisterAsync(request.Id, request.DisplayName, request.IsActive);
            var response = new RegisterResponse
            {
                Id = register.Id.ToString(),
                DisplayName = register.DisplayName,
                DocumentCount = register.Documents.Count,
                IsActive = register.IsActive
            };
            return Ok(response);
        }

        [HttpPut("order")]
        public async Task<ActionResult> OrderRegister([FromBody]OrderRegisterRequest request)
        {
            var ids = request.OrderEntries.Select(req => req.Id);

            var registers = await _documentlibContext.Registers
                .Include(reg => reg.Documents)
                .Where(reg => ids.Contains(reg.Id))
                .ToListAsync();

            foreach (var register in registers)
            {
                var entry = request.OrderEntries.Single(x => x.Id == register.Id);
                entry.Order = register.Order;
            }

            _documentlibContext.Update(registers);

            await _documentlibContext.SaveChangesAsync();

            var response = registers.OrderBy(reg => reg.Order).Select(reg => new RegisterResponse
            {
                Id = reg.Id.ToString(),
                DocumentCount = reg.Documents.Count,
                DisplayName = reg.DisplayName,
                Order = reg.Order,
                IsActive = reg.IsActive
            });
            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFolder(Guid id)
        {
            var folder = await _documentlibContext.Folders
                .Include(f => f.Registers)
                .ThenInclude(registers => registers.Documents)
                .SingleOrDefaultAsync(f => f.Id == id);

            var documents = folder.Registers.SelectMany(reg => reg.Documents);

            _documentlibContext.LibDocuments.RemoveRange(documents);
            _documentlibContext.Registers.RemoveRange(folder.Registers);
            _documentlibContext.Folders.Remove(folder);
            await _documentlibContext.SaveChangesAsync();

            return Ok("deleted");
        }
    }

    public partial class FolderController
    {
        public class RegisterResponse
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public int DocumentCount { get; set; }
            public int Order { get; set; }
            public bool IsActive { get; set; }
        }

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

        public class PutRegisterRequest
        {
            [JsonProperty("id")]
            public Guid Id { get; set; }

            [JsonProperty("displayName")]
            public string DisplayName { get; set; }

            [JsonProperty("isActive")]
            public bool? IsActive { get; set; }
        }

        public class OrderRegisterRequest
        {
            [JsonProperty("orderEntries")]
            public OrderRegisterEntry[] OrderEntries { get; set; }
        }

        public class OrderRegisterEntry
        {
            [JsonProperty("id")]
            public Guid Id { get; set; }

            [JsonProperty("order")]
            public int Order { get; set; }
        }
    }
}