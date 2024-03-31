using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Interfaces;

public interface ITagService
{
    Task<List<TagModel>> GetTagsAsync();
    Task<List<TagModel>> GetOrCreateTagsAsync(List<string> tags);
    Task<TagModel> GetOrCreateTagAsync(string tag);
}