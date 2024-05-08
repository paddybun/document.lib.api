using document.lib.shared.Models.Data;

namespace document.lib.shared.Interfaces;

public interface ITagService
{
    Task<TagModel?> GetTagByNameAsync(string name);
    Task<TagModel?> GetTagByIdAsync(int id);
    Task<(int, List<TagModel>)> GetTagsPagedAsync(int page, int pageSize);

    Task<List<TagModel>> GetTagsAsync();
    Task<List<TagModel>> GetOrCreateTagsAsync(List<string> tags);
    Task<TagModel> GetOrCreateTagAsync(string tag);
}