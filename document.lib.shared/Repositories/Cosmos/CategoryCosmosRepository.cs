using document.lib.shared.Constants;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Repositories.Cosmos;

public class CategoryCosmosRepository : ICategoryRepository
{
    private readonly Container _cosmosContainer;

    public CategoryCosmosRepository(IOptions<AppConfiguration> config)
    {
        var cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        var db = cosmosClient.GetDatabase(TableNames.Doclib);
        _cosmosContainer = db.GetContainer(TableNames.Doclib);
    }

    public async Task<DocLibCategory> GetCategoryByNameAsync(string categoryName)
    {
        return await GetCategoryByIdAsync($"Category.{categoryName}");
    }

    public async Task<DocLibCategory> GetCategoryByIdAsync(string id)
    {
        var category = _cosmosContainer.GetItemLinqQueryable<DocLibCategory>(true)
            .Where(x => x.Id == id)
            .AsEnumerable()
            .FirstOrDefault();
        return await Task.FromResult(category);
    }

    public async Task<DocLibCategory> CreateCategoryAsync(DocLibCategory category)
    {
        var id = $"Category.{category.Name}";
        var cat = new DocLibCategory
        {
            Id = id,
            Name = category.Name,
            DisplayName = category.DisplayName,
            Description = ""
        };
        var response = await _cosmosContainer.CreateItemAsync(cat);
        return response.Resource;
    }
}