using System;
using System.Linq;
using System.Threading.Tasks;
using document.lib.functions.TableEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Extensions.Logging;

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
                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsDocumentLibStorage");
                var csa = CloudStorageAccount.Parse(connectionString);
                var tbs = csa.CreateCloudTableClient(new TableClientConfiguration());
                var tableName = "doclib";
                var table = tbs.GetTableReference(tableName);
                var query = table.CreateQuery<DocLibDocument>().Where(x => x.PartitionKey.Equals("unsorted") && x.Unsorted).AsTableQuery();
                var results = table.ExecuteQuery(query);
                return new OkObjectResult(results.Select(x => x.PhysicalName).ToArray());
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
            
        }
    }
}
