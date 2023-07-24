using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.TableEntities;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

// Scoped injection 
public class TagSqlRepository : ITagRepository, IDisposable
{
    private readonly DocumentLibContext _context;

    public TagSqlRepository(DocumentLibContext context)
    {
        _context = context;
    }

    public async Task<DocLibTag> GetTagByNameAsync(string tagName)
    {
        var tag = await _context.Tags
            .SingleOrDefaultAsync(x => x.Name == tagName);
        return Map(tag);
    }

    public async Task<DocLibTag> GetTagByIdAsync(string id)
    {
        var parsedId = int.Parse(id);
        var tag = await _context.Tags
            .SingleOrDefaultAsync(x => x.Id == parsedId);
        return Map(tag);
    }

    public async Task<DocLibTag> CreateTagAsync(string tagName)
    {
        var tag = new EfTag {Name = tagName, DisplayName = tagName};
        await _context.AddAsync(tag);
        await _context.SaveChangesAsync();
        return Map(tag);

    }

    private DocLibTag Map(EfTag efTag)
    {
        if (efTag == null) return null;
        return new DocLibTag
        {
            Id = efTag.Id.ToString(),
            Name = efTag.Name,
            DisplayName = efTag.DisplayName
        };
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}