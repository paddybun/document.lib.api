using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;

namespace document.lib.shared.Interfaces;

public interface ITagService
{
    Task<ITypedServiceResult<TagModel>> GetTagByIdAsync(int id);
    Task<ITypedServiceResult<TagModel>> GetTagByNameAsync(string name);
    Task<ITypedServiceResult<(int, List<TagModel>)>> GetTagsPagedAsync(int page, int pageSize);
    Task<ITypedServiceResult<List<TagModel>>> GetTagsAsync();
    Task<ITypedServiceResult<List<TagModel>>> GetOrCreateTagsAsync(List<string> tags);
    Task<ITypedServiceResult<TagModel>> GetOrCreateTagAsync(string tag);
}