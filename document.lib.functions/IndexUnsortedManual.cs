using System;
using System.Threading.Tasks;
using document.lib.functions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace document.lib.functions
{
    public static class IndexUnsortedManual
    {
        [Disable("TriggeredByBlob")]
        [FunctionName("IndexUnsortedManual")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var indexerService = new IndexerService();
                var result = await indexerService.IndexUnsortedAsync();
                return new OkObjectResult($"Indexed {result.Count} unsorted documents");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
