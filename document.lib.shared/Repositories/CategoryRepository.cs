using document.lib.shared.Constants;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly Container _cosmosContainer;

    public CategoryRepository(IOptions<AppConfiguration> config)
    {
        var cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        var db = cosmosClient.GetDatabase(TableNames.Doclib);
        _cosmosContainer = db.GetContainer(TableNames.Doclib);
    }

    public DocLibCategory GetCategoryByName(string categoryName)
    {
        return GetCategoryById($"Category.{categoryName}");
    }

    public DocLibCategory GetCategoryById(string id)
    {
        var category = _cosmosContainer.GetItemLinqQueryable<DocLibCategory>(true)
            .Where(x => x.Id == id)
            .AsEnumerable()
            .FirstOrDefault();

        return category;
    }

    public async Task<DocLibCategory> CreateCategoryAsync(string category)
    {
        var id = $"Category.{category}";
        var cat = new DocLibCategory
        {
            Id = id,
            Name = category,
            Description = ""
        };
        var response = await _cosmosContainer.CreateItemAsync(cat);
        return response.Resource;
    }
}