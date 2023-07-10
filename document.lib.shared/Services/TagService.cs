using document.lib.shared.Interfaces;
using document.lib.shared.TableEntities;

namespace document.lib.shared.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _repository;

    public TagService(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DocLibTag>> CreateOrGetTagsAsync(string[] tags)
    {
        var toReturn = new List<DocLibTag>();
        foreach (var tag in tags)
        {
            toReturn.Add(await CreateOrGetTagAsync(tag));
        }

        return toReturn;
    }

    public async Task<DocLibTag> CreateOrGetTagAsync(string tag)
    {
        var tagEntity = await _repository.GetTagByNameAsync(tag);
        if (tagEntity == null)
        {
            return await _repository.CreateTagAsync(tag);
        }

        return tagEntity;
    }
}