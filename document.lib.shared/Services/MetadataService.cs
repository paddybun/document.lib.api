using document.lib.shared.Constants;
using document.lib.shared.Helper;
using document.lib.shared.Models;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Services;

public class MetadataService
{
    private readonly CosmosClient _cosmosClient;

    public MetadataService(IOptions<AppConfiguration> config)
    {
        _cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
    }

    public async Task<MetadataResponse> GetMetadataAsync()
    {
        var db = _cosmosClient.GetDatabase(TableNames.Doclib);
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

        return response;
    }
}