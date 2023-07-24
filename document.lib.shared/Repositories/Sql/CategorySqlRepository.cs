using System.Net.Quic;
using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.TableEntities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

// Scoped injection
public class CategorySqlRepository: ICategoryRepository
{
    private readonly DocumentLibContext _context;

    public CategorySqlRepository(DocumentLibContext context)
    {
        _context = context;
    }

    public async Task<DocLibCategory> GetCategoryByIdAsync(string id)
    {
        var parsedId = int.Parse(id);
        var efCategory = await _context.Categories.SingleOrDefaultAsync(x => x.Id == parsedId);
        return Map(efCategory);
    }

    public async Task<DocLibCategory> GetCategoryByNameAsync(string categoryName)
    {
        var efCategory = await _context.Categories.SingleOrDefaultAsync(x => x.Name == categoryName);
        return Map(efCategory);
    }

    public async Task<DocLibCategory> GetCategoryAsync(CategoryQueryParameters queryParameters)
    {
        Category efCategory;
        if (queryParameters.Id.HasValue)
        {
            efCategory = await _context.Categories.SingleOrDefaultAsync(x => x.Id == queryParameters.Id.Value);
        }
        else
        {
            efCategory = await _context.Categories.SingleOrDefaultAsync(x => x.Name == queryParameters.Name);
        }
        return Map(efCategory);
    }

    public async Task<DocLibCategory> CreateCategoryAsync(DocLibCategory category)
    {
        var efCategory = new Category
        {
            Name = category.Name,
            Description = category.Description,
            DisplayName = category.DisplayName
        };
        await _context.Categories.AddAsync(efCategory);
        await _context.SaveChangesAsync();
        return Map(efCategory);
    }

    private DocLibCategory Map(Category category)
    {
        if (category == null) return null;
        return new DocLibCategory
        {
            Name = category.Name,
            Description = category.Description,
            Id = category.Id.ToString()
        };
    }
}