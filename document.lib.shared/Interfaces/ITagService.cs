using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface ITagService
{
    Task<List<DocLibTag>> GetOrCreateTagsAsync(string[] tags);
    Task<DocLibTag> GetOrCreateTagAsync(string tag);
}