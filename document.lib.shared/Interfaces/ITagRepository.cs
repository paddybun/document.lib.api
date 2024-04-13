using document.lib.shared.Models.Models;
using document.lib.shared.Models.QueryDtos;

namespace document.lib.shared.Interfaces;

public interface ITagRepository
{
    Task<TagModel?> GetTagAsync(TagQueryParameters queryParameters);
    Task<List<TagModel>> GetTagsAsync();
    Task<TagModel> CreateTagAsync(TagModel tagName);
}