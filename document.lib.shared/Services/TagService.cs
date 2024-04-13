using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;

namespace document.lib.shared.Services;

public class TagService(ITagRepository repository) : ITagService
{
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