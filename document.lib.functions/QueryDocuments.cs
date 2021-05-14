using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using document.lib.functions.Services;
using document.lib.functions.TableEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace document.lib.functions
{
    public static class QueryDocuments
    {
        [FunctionName("QueryDocuments")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                await req.ReadAsStringAsync();
                using var sr = new StreamReader(req.Body);
                var obj = await sr.ReadToEndAsync();
                var query = JsonConvert.DeserializeObject<DocumentQuery>(obj);
                var queryService = new QueryService();
                var results = await queryService.ExecuteQueryAsync(query);
                return new OkObjectResult(results.ToList());
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
