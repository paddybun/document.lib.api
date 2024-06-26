﻿using document.lib.ef.Entities;

namespace document.lib.shared.Interfaces;

public interface ICategoryRepository<T>: IRepository
{   
    Task<T?> GetCategoryAsync(int id);
    Task<T?> GetCategoryAsync(string name);
    Task<PagedResult<T>> GetCategoriesPagedAsync(int page, int pageSize);
    Task<T> CreateCategoryAsync(EfCategory category);
}