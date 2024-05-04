using document.lib.shared.Models.Models;

namespace document.lib.shared.Interfaces;

public interface ITagRepository<T>
{
    Task<T?> GetTagByIdAsync(int id);
    Task<T?> GetTagByNameAsync(string name);
    Task<List<T>> GetTagsAsync();
    Task<(int, List<T>)> GetTagsAsync(int page, int pageSize);
    Task<T> CreateTagAsync(string name, string? displayName);
}