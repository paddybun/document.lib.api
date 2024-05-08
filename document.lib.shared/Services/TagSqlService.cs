using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Data;

namespace document.lib.shared.Services;

public class TagSqlService(ITagRepository<EfTag> repository) : ITagService
{
    public async Task<TagModel?> GetTagByIdAsync(int id)
    {
        var tag = await repository.GetTagByIdAsync(id);
        return tag == null ? null : Map(tag);
    }

    public async Task<TagModel?> GetTagByNameAsync(string name)
    {
        var tag = await repository.GetTagByNameAsync( name );
        return tag == null ? null : Map(tag);
    }

    public async Task<(int, List<TagModel>)> GetTagsPagedAsync(int page, int pageSize)
    {
        var (count, tags) = await repository.GetTagsAsync(page, pageSize);
        var mapped = tags.Select(Map).ToList();
        return (count, mapped);
    }

    public async Task<List<TagModel>> GetTagsAsync()
    {
        var tags = await repository.GetTagsAsync();
        return tags.Select(Map).ToList();
    }

    public async Task<List<TagModel>> GetOrCreateTagsAsync(List<string> tags)
    {
        var toReturn = new List<TagModel>();
        foreach (var tag in tags)
        {
            toReturn.Add(await GetOrCreateTagAsync(tag));
        }

        return toReturn;
    }

    public async Task<TagModel> GetOrCreateTagAsync(string name)
    {
        var tag = await repository.GetTagByNameAsync(name) ?? await repository.CreateTagAsync(name, name);
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