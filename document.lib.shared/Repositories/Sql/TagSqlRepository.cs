using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

// Scoped injection 
public sealed class TagSqlRepository(DocumentLibContext context) : ITagRepository
{
    public async Task<TagModel?> GetTagAsync(TagModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Id) && string.IsNullOrWhiteSpace(model.Name))
            throw new InvalidParameterException(model.GetType());

        EfTag? efTag;
        if (!string.IsNullOrWhiteSpace(model.Id))
        {
            var id = int.Parse(model.Id);
            efTag = await context.Tags
                .SingleOrDefaultAsync(x => x.Id == id);    
        }
        else
        {
            efTag = await context.Tags
                .SingleOrDefaultAsync(x => x.Name == model.Name);
        }

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