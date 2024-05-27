namespace document.lib.shared.Interfaces;

public interface ITagRepository<T>: IRepository
{
    Task<T?> GetTagAsync(int id);
    Task<T?> GetTagAsync(string name);
    Task<List<T>> GetTagsAsync(string[] names);
    Task<(int, List<T>)> GetTagsAsync(int page, int pageSize);
    Task<List<T>> CreateTagsAsync(params T[] tags);
}