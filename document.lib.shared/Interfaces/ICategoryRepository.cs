﻿using document.lib.shared.Models.QueryDtos;
using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface ICategoryRepository
{
    Task<DocLibCategory> GetCategoryAsync(CategoryQueryParameters queryParameters);
    Task<List<DocLibCategory>> GetCategoriesAsync();
    Task<DocLibCategory> CreateCategoryAsync(DocLibCategory category);
}