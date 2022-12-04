using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface ITagService
{
    IAsyncEnumerable<DocLibTag> CreateOrGetTagsAsync(string[] tags);
    Task<DocLibTag> CreateOrGetTagAsync(string tag);
}