using document.lib.shared.Constants;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.Data;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Repositories.Cosmos;

public class TagCosmosRepository : ITagRepository<DocLibTag>
{
    private readonly Container _cosmosContainer;

    public TagCosmosRepository(IOptions<SharedConfig> config)
    {
        var cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        var db = cosmosClient.GetDatabase(TableNames.Doclib);
        _cosmosContainer = db.GetContainer(TableNames.Doclib);
    }

    public Task<DocLibTag?> GetTagAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<DocLibTag?> GetTagAsync(string name)
    {
        var tag = _cosmosContainer.GetItemLinqQueryable<DocLibTag>(true)
            .SingleOrDefault(x => x.Name == name);
        return await Task.FromResult(tag);
    }

    public async Task<List<DocLibTag>> GetTagsAsync(params string[] names)
    {
        var tags = _cosmosContainer.GetItemLinqQueryable<DocLibTag>(true)
            .Where(x => x.Id.StartsWith("Tag."))
            .AsEnumerable()
            .ToList();
        return await Task.FromResult(tags.ToList());
    }

    public async Task<(int, List<DocLibTag>)> GetTagsAsync(int page, int pageSize)
    {
        var tags = _cosmosContainer.GetItemLinqQueryable<DocLibTag>(true)
            .Where(x => x.Id.StartsWith("Tag."))
            .AsEnumerable()
            .ToList();
        return await Task.FromResult((tags.Count, tags));
    }

    public Task<List<DocLibTag>> CreateTagsAsync(params DocLibTag[] tags)
    {
        throw new NotImplementedException();
    }

    public async Task<DocLibTag> CreateTagAsync(string name, string? displayName)
    {
        var id = $"Tag.{name}";
        var lowercased = name.ToLower();
        var response = await _cosmosContainer.CreateItemAsync(new DocLibTag
        {
            Id = id,
            Name = lowercased,
        });

        return response.Resource;
    }

    public Task SaveAsync()
    {
        throw new NotImplementedException();
    }

    private static TagModel Map(DocLibTag tag)
    {
        return new TagModel
        {
            Name = tag.Name,
            DisplayName = tag.DisplayName,
            Id = tag.Id
        };
    }
}