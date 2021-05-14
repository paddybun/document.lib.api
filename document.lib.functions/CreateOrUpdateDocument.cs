using System;
using System.IO;
using System.Threading.Tasks;
using document.lib.functions.Services;
using document.lib.functions.TableEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace document.lib.functions
{
    public static class CreateOrUpdateDocument
    {
        [FunctionName("CreateOrUpdateDocument")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                using var streamReader = new StreamReader(req.Body);
                var requestBody = await streamReader.ReadToEndAsync();
                var docLibDocument = JsonConvert.DeserializeObject<DocLibDocument>(requestBody);
                var docService = new DocLibService();
                await docService.CreateDocLibDocumentAsync(docLibDocument);
                return new OkObjectResult(docLibDocument);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}

