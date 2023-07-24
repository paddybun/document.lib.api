using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _repository;

    public TagService(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TagModel>> GetOrCreateTagsAsync(string[] tags)
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
        var tagEntity = await _repository.GetTagAsync(new TagQueryParameters(name: tag));
        if (tagEntity == null)
        {
            return await _repository.CreateTagAsync(tag);
        }

        return tagEntity;
    }
}