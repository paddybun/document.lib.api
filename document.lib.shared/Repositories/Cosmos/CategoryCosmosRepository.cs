using document.lib.shared.Constants;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.Models;
using document.lib.shared.Models.QueryDtos;
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

    public async Task<CategoryModel?> GetCategoryAsync(CategoryQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (!queryParameters.IsValid()) throw new InvalidQueryParameterException(queryParameters.GetType());

        DocLibCategory? category;
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

        if (category == null)
        {
            return null;
        }

        return await Task.FromResult(MapToModel(category));
    }

    public async Task<List<CategoryModel>> GetCategoriesAsync()
    {
        var categories = _cosmosContainer.GetItemLinqQueryable<DocLibCategory>(true)
            .Where(x => x.Id.StartsWith("Category."))
            .AsEnumerable()
            .ToList();
        return await Task.FromResult(categories.Select(MapToModel).ToList());
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
        var response = await _cosmosContainer.CreateItemAsync(cat, new PartitionKey(cat.Id));
        return MapToModel(response.Resource);
    }

    public async Task<CategoryModel> UpdateCategoryAsync(CategoryModel category)
    {
        var entity = MapToEntity(category);
        await _cosmosContainer.UpsertItemAsync(entity, new PartitionKey(entity.Id));
        return MapToModel(entity);
    }

    private static DocLibCategory MapToEntity(CategoryModel categoryModel)
    {
        return new DocLibCategory
        {
            Id = categoryModel.Id,
            Name = categoryModel.Name,
            DisplayName = categoryModel.DisplayName,
            Description = categoryModel.Description
        };
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