using document.lib.shared.Models.QueryDtos;
using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface ITagRepository
{
    Task<DocLibTag> GetTagAsync(TagQueryParameters queryParameters);
    Task<List<DocLibTag>> GetTagsAsync();
    Task<DocLibTag> CreateTagAsync(string tagName);
}