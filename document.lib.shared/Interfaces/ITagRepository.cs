using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface ITagRepository
{
    Task<DocLibTag> GetTagByNameAsync(string tagName);
    Task<DocLibTag> GetTagByIdAsync(string id);
    Task<DocLibTag> CreateTagAsync(string tagName);
}