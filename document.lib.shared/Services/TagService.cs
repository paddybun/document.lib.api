using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;

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
            return await repository.CreateTagAsync(tag);
        }

        return tagEntity;
    }
}