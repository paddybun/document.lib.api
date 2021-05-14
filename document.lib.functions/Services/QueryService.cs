using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using document.lib.functions.Constants;
using document.lib.functions.Helper;
using document.lib.functions.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace document.lib.functions.Services
{
    public class QueryService
    {
        private readonly CosmosClient _cosmosClient;
        public QueryService()
        {
            var cosmosConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsCosmos");
            _cosmosClient = new CosmosClient(cosmosConnectionString);
        }

        public async Task<IEnumerable<DocLibDocument>> ExecuteQueryAsync(DocumentQuery query)
        {
            var db = _cosmosClient.GetDatabase(TableNames.Doclib);
            var container = db.GetContainer(TableNames.Doclib);

            var sb = new StringBuilder();
            sb.Append("SELECT * FROM doclib dl WHERE ");
            if (query.Unsorted)
            {
                sb.AppendLine("dl.unsorted = true ");
                return await CosmosQueryHelper.ExecuteQueryAsync<DocLibDocument>(new QueryDefinition(sb.ToString()), container);
            }

            var andPredicates = new List<string>();
            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                andPredicates.Add($"dl.category = '{query.Category}'");
            }
            if (!string.IsNullOrWhiteSpace(query.Description))
            {
                andPredicates.Add($"dl.description LIKE '%{query.Description}%'");
            }
            if (!string.IsNullOrWhiteSpace(query.Company))
            {
                andPredicates.Add($"dl.company = '{query.Company}'");
            }
            if (!string.IsNullOrWhiteSpace(query.DisplayName))
            {
                andPredicates.Add($"dl.displayName = '{query.DisplayName}'");
            }
            if (!string.IsNullOrWhiteSpace(query.PhysicalName))
            {
                andPredicates.Add($"dl.physicalName LIKE '%{query.PhysicalName}%'");
            }
            if (query.Tags?.Length > 0)
            {
                var tagQueries = new List<string>();
                foreach (var tag in query.Tags.Select(x => x.ToLower()))
                {
                    tagQueries.Add($"ARRAY_CONTAINS(dl.tags, '{tag}')");
                }
                andPredicates.Add($"({string.Join(" OR ", tagQueries).Trim()})");
            }
            if (query.UploadDateFrom != null)
            {
                var start = query.UploadDateFrom.Value.ToString("yyyy-MM-ddT00:00:00");
                var end = query.UploadDateTo?.ToString("yyyy-MM-ddT00:00:00") ?? query.UploadDateFrom.Value.AddDays(1).ToString("yyyy-MM-ddT00:00:00");
                andPredicates.Add($"(dl.uploadDate BETWEEN '{start}' AND '{end}')");
            }
            if (query.DateOfDocumentFrom != null)
            {
                var start = query.DateOfDocumentFrom.Value.ToString("yyyy-MM-ddT00:00:00");
                var end = query.DateOfDocumentTo?.ToString("yyyy-MM-ddT00:00:00") ?? query.DateOfDocumentFrom.Value.AddDays(1).ToString("yyyy-MM-ddT00:00:00");
                andPredicates.Add($"(dl.dateOfDocument BETWEEN '{start}' AND '{end}')");
            }

            var combined = string.Join(" AND ", andPredicates);
            sb.AppendLine(combined.Trim());
            return await CosmosQueryHelper.ExecuteQueryAsync<DocLibDocument>(new QueryDefinition(sb.ToString()), container);
        }
    }
    public class DocumentQuery
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("physicalName")]
        public string PhysicalName { get; set; }

        [JsonProperty("uploadDateFrom")]
        public DateTimeOffset? UploadDateFrom { get; set; }

        [JsonProperty("uploadDateTo")]
        public DateTimeOffset? UploadDateTo { get; set; }

        [JsonProperty("dateOfDocumentFrom")]
        public DateTimeOffset? DateOfDocumentFrom { get; set; }

        [JsonProperty("dateOfDocumentTo")]
        public DateTimeOffset? DateOfDocumentTo { get; set; }

        [JsonProperty("unsorted")]
        public bool Unsorted { get; set; }
    }
}
