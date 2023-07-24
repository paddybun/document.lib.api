using System.Net.Quic;
using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Exceptions;
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

    public async Task<DocLibCategory> GetCategoryAsync(CategoryQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (!queryParameters.IsValid()) throw new InvalidQueryParameterException(queryParameters.GetType());

        EfCategory efCategory;
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

    public async Task<List<DocLibCategory>> GetCategoriesAsync()
    {
        var categories = await _context.Categories.ToListAsync();
        return categories.Select(Map).ToList();
    }

    public async Task<DocLibCategory> CreateCategoryAsync(DocLibCategory category)
    {
        var efCategory = new EfCategory
        {
            Name = category.Name,
            Description = category.Description,
            DisplayName = category.DisplayName
        };
        await _context.Categories.AddAsync(efCategory);
        await _context.SaveChangesAsync();
        return Map(efCategory);
    }

    // TODO: Refactor to corresponding mapper
    private static DocLibCategory Map(EfCategory efCategory)
    {
        if (efCategory == null) return null;
        return new DocLibCategory
        {
            Name = efCategory.Name,
            Description = efCategory.Description,
            Id = efCategory.Id.ToString()
        };
    }
}