using document.lib.shared.Constants;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Repositories.Cosmos;

public class TagCosmosRepository : ITagRepository
{
    private readonly Container _cosmosContainer;

    public TagCosmosRepository(IOptions<AppConfiguration> config)
    {
        var cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        var db = cosmosClient.GetDatabase(TableNames.Doclib);
        _cosmosContainer = db.GetContainer(TableNames.Doclib);
    }

    public async Task<DocLibTag> GetTagByNameAsync(string tagName)
    {
        var tag = await GetTagByIdAsync($"Tag.{tagName}");
        return tag;
    }

    public async Task<DocLibTag> GetTagByIdAsync(string id)
    {
        var tag = _cosmosContainer.GetItemLinqQueryable<DocLibTag>(true)
            .Where(x => x.Id == id)
            .AsEnumerable()
            .FirstOrDefault();
        return await Task.FromResult(tag);
    }
    
    public async Task<DocLibTag> CreateTagAsync(string tagName)
    {
        var id = $"Tag.{tagName}";
        var lowercased = tagName;
        var response = await _cosmosContainer.CreateItemAsync(new DocLibTag
        {
            Id = id,
            Name = lowercased,
        });
        return response.Resource;
    }
}