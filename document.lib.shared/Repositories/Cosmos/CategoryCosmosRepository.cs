using document.lib.shared.Constants;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;
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

    public async Task<CategoryModel> GetCategoryAsync(CategoryQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (!queryParameters.IsValid()) throw new InvalidQueryParameterException(queryParameters.GetType());

        DocLibCategory category;
        if (queryParameters.Id.HasValue)
        {
            category = _cosmosContainer.GetItemLinqQueryable<DocLibCategory>(true)
                .Where(x => x.Id == queryParameters.Id.Value.ToString())
                .AsEnumerable()
                .FirstOrDefault();
        }
        else
        {
            category = _cosmosContainer.GetItemLinqQueryable<DocLibCategory>(true)
                .Where(x => x.Id == $"Category.{queryParameters.Name}")
                .AsEnumerable()
                .FirstOrDefault();
        }
        return await Task.FromResult(Map(category));
    }

    public async Task<List<CategoryModel>> GetCategoriesAsync()
    {
        var categories = _cosmosContainer.GetItemLinqQueryable<DocLibCategory>(true)
            .Where(x => x.Id == $"Category.")
            .AsEnumerable()
            .ToList();
        return await Task.FromResult(categories.Select(Map).ToList());
    }

    public async Task<CategoryModel> CreateCategoryAsync(CategoryModel category)
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
        return Map(response.Resource);
    }

    private static CategoryModel Map(DocLibCategory category)
    {
        if (category == null)
        {
            return null;
        }

        return new CategoryModel
        {
            Name = category.Name,
            DisplayName = category.DisplayName,
            Description = category.Description,
            Id = category.Id
        };
    }
}