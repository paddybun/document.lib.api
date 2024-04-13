using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface ITagRepository
{
    Task<TagModel?> GetTagAsync(TagModel tagModel);
    Task<List<TagModel>> GetTagsAsync();
    Task<TagModel> CreateTagAsync(TagModel tagName);
}