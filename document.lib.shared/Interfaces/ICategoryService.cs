using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;

namespace document.lib.shared.Interfaces;

public interface ICategoryService
{
    Task<ITypedServiceResult<CategoryModel>> GetCategoryAsync(int id);
    Task<ITypedServiceResult<CategoryModel>> GetCategoryAsync(string name);
    Task<ITypedServiceResult<PagedResult<CategoryModel>>> GetCategoriesPagedAsync(int page, int pageSize);
    Task<ITypedServiceResult<CategoryModel>> CreateCategoryAsync(CategoryModel model);
    Task<ITypedServiceResult<CategoryModel>> UpdateCategory(CategoryModel model);
}