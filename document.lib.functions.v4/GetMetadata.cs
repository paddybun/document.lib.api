using System;
using System.Threading.Tasks;
using document.lib.shared.Constants;
using document.lib.shared.Helper;
using document.lib.shared.TableEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace document.lib.functions.v4
{
    public static class GetMetadata
    {
        [Disable("DisableFunction")]
        [FunctionName("GetMetadataAsync")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var cosmosConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsCosmos");
                var cosmosClient = new CosmosClient(cosmosConnectionString);
                var db = cosmosClient.GetDatabase(TableNames.Doclib);
                var docLibContainer = db.GetContainer(TableNames.Doclib);

                var tagQuery = "SELECT * FROM doclib dl WHERE dl.id LIKE 'Tag.%'";
                var categoryQuery = "SELECT * FROM doclib dl WHERE dl.id LIKE 'Category.%'";
                var folderQuery = "SELECT * FROM doclib dl WHERE dl.id LIKE 'Folder.%'";

                var tags = await CosmosQueryHelper.ExecuteQueryAsync<DocLibTag>(new QueryDefinition(tagQuery), docLibContainer);
                var categories = await CosmosQueryHelper.ExecuteQueryAsync<DocLibCategory>(new QueryDefinition(categoryQuery), docLibContainer);
                var folders = await CosmosQueryHelper.ExecuteQueryAsync<DocLibFolder>(new QueryDefinition(folderQuery), docLibContainer);

                var response = new MetadataResponse
                {
                    Tags = tags.ToArray(),
                    Categories = categories.ToArray(),
                    Folders = folders.ToArray()
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
            
        }
    }

    public class MetadataResponse
    {
        [JsonProperty("tags")]
        public DocLibTag[] Tags { get; set; }
        [JsonProperty("categories")]
        public DocLibCategory[] Categories { get; set; }
        [JsonProperty("folders")]
        public DocLibFolder[] Folders { get; set; }
    }
}
