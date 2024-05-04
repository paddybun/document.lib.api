using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class TagSqlRepository(DocumentLibContext context) : ITagRepository<EfTag>
{
    public async Task<EfTag?> GetTagByIdAsync(int id)
    {
        var efTag = await context.Tags.SingleOrDefaultAsync(x => x.Id == id);
        return efTag;
    }

    public async Task<EfTag?> GetTagByNameAsync(string name)
    {
        var efTag = await context.Tags.SingleOrDefaultAsync(x => x.Name == name);
        return efTag;
    }

    public async Task<List<EfTag>> GetTagsAsync()
    {
        var tags = await context.Tags.ToListAsync();
        return tags.ToList();
    }

    public async Task<(int, List<EfTag>)> GetTagsAsync(int page, int pageSize)
    {
        var count = await context.Tags.CountAsync();
        var tags = await context.Tags
            .OrderBy(x => x.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedTags = tags.ToList();
        return (count, mappedTags);
    }

    public async Task<EfTag> CreateTagAsync(string name, string? displayName)
    {
        var tag = new EfTag {Name = name, DisplayName = displayName};
        await context.AddAsync(tag);
        await context.SaveChangesAsync();
        return tag;
    }    
}