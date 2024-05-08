namespace document.lib.shared.Interfaces;

public interface ICategoryRepository<T>
{   
    Task<T?> GetCategoryByIdAsync(int id);
    Task<T?> GetCategoryByNameAsync(string name);
    Task<List<T>> GetCategoriesAsync();
    Task<T> CreateCategoryAsync(string name, string? description = null, string? displayName = null);
    Task<T> UpdateCategoryAsync(T category);
}