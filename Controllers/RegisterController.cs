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
    public partial class RegisterController : Controller
    {
        private readonly DocumentlibContext _dbContext;

        public RegisterController(DocumentlibContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult GetRegisters()
        {
            var registers = _dbContext.Registers.OrderBy(reg => reg.Id);
            return Ok(registers);
        }

        [HttpGet("{id}")]
        public ActionResult GetRegister(Guid id)
        {
            var register = _dbContext.Registers
                .Include(reg => reg.Folder)
                .Include(reg => reg.Documents)
                .Single(reg => reg.Id == id);

            var response = Responses.FullRegisterResponse.CreateResponse(register);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateRegister([FromBody] Responses.PostRegisterRequest request)
        {
            var register = _dbContext.Registers
                .Include(reg => reg.Folder)
                .Include(reg => reg.Documents)
                .Single(reg => reg.Id == request.Id);

            var folder = _dbContext.Folders.Single(f => f.Id == request.FolderId);
            register.Folder = folder;
            register.Name = request.Name;
            register.DocumentCount = request.Count;

            _dbContext.Registers.Update(register);
            await _dbContext.SaveChangesAsync();

            var response = Responses.FullRegisterResponse.CreateResponse(register);
            return Ok(response);
        }
        [HttpPut]
        public async Task<ActionResult> CreateRegister([FromBody] Responses.PutRegisterRequest request)
        {
            var folder = _dbContext.Folders.Single(f => f.Id == request.FolderId);
            var register = new Register
            {
                Folder = folder,
                Name = request.Name,
                DocumentCount = request.Count
            };
            await _dbContext.Registers.AddAsync(register);
            await _dbContext.SaveChangesAsync();

            var response = Responses.FullRegisterResponse.CreateResponse(register);
            return Ok(response);
        }
    }

    public class Responses
    {
        public class FullRegisterResponse
        {
            public static FullRegisterResponse CreateResponse(Register register)
            {
                var response = new FullRegisterResponse
                {
                    Id = register.Id.ToString(),
                    Name = register.Name,
                    Count = register.DocumentCount,
                    Folder = new GetFolderResponse
                    {
                        Id = register.Folder.Id.ToString(),
                        Name = register.Folder.Name
                    }
                };

                return response;
            }

            public string Id { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
            public GetFolderResponse Folder{ get; set; }
        }

        public class GetFolderResponse
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class PutRegisterRequest
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("count")]
            public int Count { get; set; }

            [JsonProperty("folder")]
            public Guid FolderId { get; set; }
        }

        public class PostRegisterRequest : PutRegisterRequest
        {
            [JsonProperty("id")]
            public Guid Id { get; set; }
        }
    }
}