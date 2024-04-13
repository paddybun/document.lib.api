using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using document.lib.shared.Models.QueryDtos;

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
        var tagEntity = await repository.GetTagAsync(new TagQueryParameters(name: tag));
        if (tagEntity == null)
        {
            var model = new TagModel { Name = tag, DisplayName = tag};
            return await repository.CreateTagAsync(model);
        }

        return tagEntity;
    }
}