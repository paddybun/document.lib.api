using document.lib.shared.Constants;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.Models;
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

    public async Task<TagModel?> GetTagAsync(TagModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            throw new InvalidParameterException("Tag query in cosmos repository only allows searching by name. Name should look like Tag.<id>");

        var id = model.Name.Split('.').Last();
        var tag = _cosmosContainer.GetItemLinqQueryable<DocLibTag>(true)
            .Where(x => x.Id == $"Tag.{id}")
            .AsEnumerable()
            .FirstOrDefault();

        return await Task.FromResult(Map(tag));
    }

    public async Task<List<TagModel>> GetTagsAsync()
    {
        var tags = _cosmosContainer.GetItemLinqQueryable<DocLibTag>(true)
            .Where(x => x.Id.StartsWith("Tag."))
            .AsEnumerable()
            .ToList();

        return await Task.FromResult(tags.Select(Map).ToList());
    }

    public async Task<TagModel> CreateTagAsync(TagModel model)
    {
        var id = $"Tag.{model.Name}";
        var lowercased = model.Name;
        var response = await _cosmosContainer.CreateItemAsync(new DocLibTag
        {
            Id = id,
            Name = lowercased,
        });

        return Map(response.Resource);
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