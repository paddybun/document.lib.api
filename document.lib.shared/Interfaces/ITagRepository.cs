using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface ITagRepository
{
    DocLibTag GetTagByName(string tagName);
    DocLibTag GetTagById(string id);
    Task<DocLibTag> CreateTagAsync(string tagName);
}