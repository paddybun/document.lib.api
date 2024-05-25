using document.lib.ef.Entities;
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

    public Task SaveAsync()
    {
        throw new NotImplementedException();
    }

    public Task<DocLibCategory?> GetCategoryAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<DocLibCategory?> GetCategoryAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<DocLibCategory>> GetCategoriesPagedAsync(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<DocLibCategory> CreateCategoryAsync(EfCategory category)
    {
        throw new NotImplementedException();
    }
}