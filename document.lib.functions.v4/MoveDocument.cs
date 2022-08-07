using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using document.lib.shared.Services;
using document.lib.shared.TableEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace document.lib.functions.v4
{
    public static class MoveDocument
    {
        [Disable("DisableFunction")]
        [FunctionName("MoveDocument")]
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
                var result = await docService.MoveDocumentAsync(docLibDocument);
                if (!result)
                {
                    return new BadRequestResult();
                }
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                return new InternalServerErrorResult();
            }
            
        }
    }
}
