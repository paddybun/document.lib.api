using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface ITagRepository
{
    Task<TagModel?> GetTagAsync(TagModel model);
    Task<List<TagModel>> GetTagsAsync();
    Task<(int, List<TagModel>)> GetTagsAsync(int page, int pageSize);
    Task<TagModel> CreateTagAsync(TagModel tagName);
}