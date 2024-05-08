using document.lib.shared.Constants;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.Data;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Repositories.Cosmos;

public class CategoryCosmosRepository : ICategoryRepository<DocLibCategory>
{
    private readonly Container _cosmosContainer;

    public CategoryCosmosRepository(IOptions<SharedConfig> config)
    {
        var cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        var db = cosmosClient.GetDatabase(TableNames.Doclib);
        _cosmosContainer = db.GetContainer(TableNames.Doclib);
    }

    public Task<DocLibCategory?> GetCategoryByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<DocLibCategory?> GetCategoryByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<List<DocLibCategory>> GetCategoriesAsync()
    {
        var categories = _cosmosContainer.GetItemLinqQueryable<DocLibCategory>(true)
            .Where(x => x.Id.StartsWith("Category."))
            .AsEnumerable()
            .ToList();
        return await Task.FromResult(categories);
    }

    public async Task<DocLibCategory> CreateCategoryAsync(string name, string? description = null, string? displayName = null)
    {
        var id = $"Category.{name}";
        var cat = new DocLibCategory
        {
            Id = id,
            Name = name,
            DisplayName = displayName,
            Description = description
        };
        var response = await _cosmosContainer.CreateItemAsync(cat, new PartitionKey(cat.Id));
        return response.Resource;
    }

    public async Task<DocLibCategory> UpdateCategoryAsync(DocLibCategory category)
    {
        await _cosmosContainer.UpsertItemAsync(category, new PartitionKey(category.Id));
        return category;
    }

    private static CategoryModel MapToModel(DocLibCategory category)
    {
        return new CategoryModel
        {
            Id = category.Id,
            Name = category.Name,
            DisplayName = category.DisplayName,
            Description = category.Description
        };
    }
}