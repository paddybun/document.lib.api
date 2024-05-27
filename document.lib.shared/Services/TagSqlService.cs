using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;

namespace document.lib.shared.Services;

public class TagSqlService(ITagRepository<EfTag> repository) : ITagService
{
    public async Task<ITypedServiceResult<TagModel>> GetTagAsync(int id)
    {
        try
        {
            var tag = await repository.GetTagAsync(id);
            return tag != null
                ? ServiceResult.Ok(Map(tag))
                : ServiceResult.DefaultError<TagModel>();
        }
        catch
        {
            return ServiceResult.DefaultError<TagModel>();
        }
        
    }

    public async Task<ITypedServiceResult<TagModel>> GetTagAsync(string name)
    {
        try
        {
            var tag = await repository.GetTagAsync(name);
            return tag != null
                ? ServiceResult.Ok(Map(tag))
                : ServiceResult.Error(default(TagModel));
        }
        catch
        {
            return ServiceResult.Error(default(TagModel));
        }
    }

    public async Task<ITypedServiceResult<(int, List<TagModel>)>> GetTagsPagedAsync(int page, int pageSize)
    {
        try
        {
            var (count, tags) = await repository.GetTagsAsync(page, pageSize);
            var mapped = tags.Select(Map).ToList();
            return ServiceResult.Ok((count, mapped));            
        }
        catch
        {
            return ServiceResult.Error<(int, List<TagModel>)>((0, []));
        }
        
    }

    public async Task<ITypedServiceResult<TagModel>> CreateTagAsync(TagModel model)
    {
        var tag = await repository.GetTagAsync(model.Name);
        if (tag != null) return ServiceResult.Ok(Map(tag));
        
        var efTag = new EfTag
        {
            Name = model.Name,
            DisplayName = model.Name
        };

        var tags = await repository.CreateTagsAsync(efTag);
        return ServiceResult.Ok(Map(tags.Single()));
    }

    public async Task<ITypedServiceResult<List<TagModel>>> CreateTagsAsync(List<TagModel> models)
    {
        var tags = await repository.GetTagsAsync(models.Select(x => x.Name).ToArray());
        var existing = tags.Select(x => x.Name);
        var newTags = models
            .Where(x => !existing.Contains(x.Name))
            .Select(x => new EfTag{ Name = x.Name, DisplayName = x.DisplayName});
        
        var createdTags = await repository.CreateTagsAsync(newTags.ToArray());
        var toReturn = tags.Concat(createdTags);
        return ServiceResult.Ok(toReturn.OrderBy(x => x.Id).Select(Map).ToList());
    }

    public async Task<ITypedServiceResult<TagModel>> UpdateTagAsync(TagModel model)
    {
        var tag = await repository.GetTagAsync(model.Name);
        if (tag == null) return ServiceResult.Error(default(TagModel));
        
        tag.DisplayName = model.DisplayName;
        await repository.SaveAsync();
        return ServiceResult.Ok(Map(tag));
    }

    public Task<ITypedServiceResult<TagModel>> DeleteTagAsync(TagModel model)
    {
        throw new NotImplementedException();
    }

    private static TagModel Map(EfTag efTag)
    {
        return new TagModel
        {
            Id = efTag.Id,
            Name = efTag.Name,
            DisplayName = efTag.DisplayName
        };
    }
}