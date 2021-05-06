using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace document.lib.functions
{
    public static class UploadDocumentFunction
    {
        [FunctionName("UploadDocumentsFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsDocumentLibStorage");
                var containerName = Environment.GetEnvironmentVariable("DocumentContainerName");
                var bcc = new BlobContainerClient(connectionString, containerName);

                await req.ReadFormAsync();
                var file = req.Form.Files["file"];
                await using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    ms.Position = 0;
                    await bcc.UploadBlobAsync($"unsorted/{file.FileName}", ms);
                }
                
                return new OkObjectResult(file.FileName + "-" + file.Length);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
