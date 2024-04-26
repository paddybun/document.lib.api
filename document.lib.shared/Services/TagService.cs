using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;

namespace document.lib.shared.Services;

public class TagService(ITagRepository repository) : ITagService
{
    public async Task<TagModel?> GetTagByIdAsync(int id)
    {
        return await repository.GetTagAsync(new TagModel { Id = id.ToString() });
    }

    public async Task<TagModel?> GetTagByNameAsync(string name)
    {
        return await repository.GetTagAsync(new TagModel { Name = name });
    }

    public async Task<(int, List<TagModel>)> GetTagsPagedAsync(int page, int pageSize)
    {
        return await repository.GetTagsAsync(page, pageSize);
    }

    public async Task<List<TagModel>> GetTagsAsync()
    {
        return await repository.GetTagsAsync();
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

    public async Task<TagModel> GetOrCreateTagAsync(string tag)
    {
        var model = new TagModel { Name = tag, DisplayName = tag};
        var tagEntity = await repository.GetTagAsync(model);

        if (tagEntity == null)
        {
            return await repository.CreateTagAsync(model);
        }

        return tagEntity;
    }
}