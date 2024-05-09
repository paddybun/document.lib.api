using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;
using document.lib.shared.Models.Result.Types;

namespace document.lib.shared.Services;

public class TagSqlService(ITagRepository<EfTag> repository) : ITagService
{
    public async Task<ITypedServiceResult<TagModel>> GetTagByIdAsync(int id)
    {
        try
        {
            var tag = await repository.GetTagByIdAsync(id);
            return tag != null
                ? ServiceResult.Ok(Map(tag))
                : ServiceResult.Error(default(TagModel));    
        }
        catch
        {
            return ServiceResult.Error(default(TagModel));
        }
        
    }

    public async Task<ITypedServiceResult<TagModel>> GetTagByNameAsync(string name)
    {
        try
        {
            var tag = await repository.GetTagByNameAsync(name);
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

    public async Task<ITypedServiceResult<List<TagModel>>> GetTagsAsync()
    {
        try
        {
            var tags = (await repository.GetTagsAsync())
                .Select(Map)
                .ToList();
            return ServiceResult.Ok(tags);
        }
        catch
        {
            return ServiceResult.Error<List<TagModel>>([]);
        }
        
    }

    public async Task<ITypedServiceResult<List<TagModel>>> GetOrCreateTagsAsync(List<string> tags)
    {
        try
        {
            var toReturn = new List<TagModel>();
            foreach (var tag in tags)
            {
                var t = await GetOrCreateTagAsync(tag);
                if (t.IsSuccess)
                    toReturn.Add(t.Data!);
            }

            return ServiceResult.Ok(toReturn);
        }
        catch
        {
            return new ErrorDataResult<List<TagModel>>([]);
        }
    }

    public async Task<ITypedServiceResult<TagModel>> GetOrCreateTagAsync(string name)
    {
        try
        {
            var tag = await repository.GetTagByNameAsync(name) ?? await repository.CreateTagAsync(name, name);
            return ServiceResult.Ok(Map(tag));
        }
        catch
        {
            return ServiceResult.Error(default(TagModel));
        }
        
    }
    
    private static TagModel Map(EfTag efTag)
    {
        return new TagModel
        {
            Id = efTag.Id.ToString(),
            Name = efTag.Name,
            DisplayName = efTag.DisplayName
        };
    }
}