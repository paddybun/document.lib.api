using document.lib.data.entities;
using document.lib.ef;
using document.lib.shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class TagSqlRepository(DocumentLibContext context) : ITagRepository<Tag>
{
    public async Task<Tag?> GetTagAsync(int id)
    {
        var efTag = await context.Tags.SingleOrDefaultAsync(x => x.Id == id);
        return efTag;
    }

    public async Task<Tag?> GetTagAsync(string name)
    {
        var efTag = await context.Tags.SingleOrDefaultAsync(x => x.Name == name);
        return efTag;
    }

    public async Task<List<Tag>> GetTagsAsync(string[] names)
    {
        var tags =await context.Tags
            .Where(x => names.Contains(x.Name))
            .ToListAsync();
        
        return tags;
    }

    public async Task<(int, List<Tag>)> GetTagsAsync(int page, int pageSize)
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

    public async Task<List<Tag>> CreateTagsAsync(params Tag[] tags)
    {
        foreach (var tag in tags)
        {
            await context.AddAsync(tag);            
        }
        await context.SaveChangesAsync();
        return tags.ToList();
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}