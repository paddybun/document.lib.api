using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Interfaces;

public interface ITagRepository
{
    Task<TagModel?> GetTagAsync(TagQueryParameters queryParameters);
    Task<List<TagModel>> GetTagsAsync();
    Task<TagModel> CreateTagAsync(TagModel tagName);
}