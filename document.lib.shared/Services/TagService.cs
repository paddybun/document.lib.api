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

    public async IAsyncEnumerable<DocLibTag> CreateOrGetTagsAsync(string[] tags)
    {
        foreach (var tag in tags)
        {
            yield return await CreateOrGetTagAsync(tag);
        }
    }

    public async Task<DocLibTag> CreateOrGetTagAsync(string tag)
    {
        var tagEntity = _repository.GetTagByName(tag);
        if (tagEntity == null)
        {
            return await _repository.CreateTagAsync(tag);
        }

        return tagEntity;
    }
}