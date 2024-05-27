using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;

namespace document.lib.shared.Interfaces;

public interface ITagService
{
    Task<ITypedServiceResult<TagModel>> GetTagAsync(int id);
    Task<ITypedServiceResult<TagModel>> GetTagAsync(string name);
    Task<ITypedServiceResult<(int, List<TagModel>)>> GetTagsPagedAsync(int page, int pageSize);
    Task<ITypedServiceResult<TagModel>> CreateTagAsync(TagModel model);
    Task<ITypedServiceResult<List<TagModel>>> CreateTagsAsync(List<TagModel> models);
    Task<ITypedServiceResult<TagModel>> UpdateTagAsync(TagModel model);
    Task<ITypedServiceResult<TagModel>> DeleteTagAsync(TagModel model);
}