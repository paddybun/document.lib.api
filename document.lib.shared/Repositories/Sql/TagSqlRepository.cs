using System.Linq.Expressions;
using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Helper;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class TagSqlRepository(DocumentLibContext context) : ITagRepository
{
    public async Task<TagModel?> GetTagAsync(TagModel model)
    {
        Expression<Func<EfTag, bool>> getTagExpression;
        
        if (PropertyChecker.Values.Any(model, x => x.Id))
            getTagExpression = x => x.Id == (int)model.Id!;
        else if (PropertyChecker.Values.Any(model, x => x.Name))
            getTagExpression = x => x.Name == model.Name;
        else
            return null;
        
        var efTag = await context.Tags.SingleOrDefaultAsync(getTagExpression);
        return efTag == null ? null : Map(efTag);
    }

    public async Task<List<TagModel>> GetTagsAsync()
    {
        var tags = await context.Tags.ToListAsync();
        return tags.Select(Map).ToList();
    }

    public async Task<(int, List<TagModel>)> GetTagsAsync(int page, int pageSize)
    {
        var count = await context.Tags.CountAsync();
        var tags = await context.Tags
            .OrderBy(x => x.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedTags = tags.Select(Map).ToList();
        return (count, mappedTags);
    }

    public async Task<TagModel> CreateTagAsync(TagModel tagModel)
    {
        var tag = new EfTag {Name = tagModel.Name, DisplayName = tagModel.DisplayName};
        await context.AddAsync(tag);
        await context.SaveChangesAsync();
        return Map(tag);
    }

    private static TagModel Map(EfTag efTag)
    {
        return new TagModel
        {
            Id = efTag.Id.ToString(),
            Name = efTag.Name,
            DisplayName = efTag.DisplayName
        };
    }
}