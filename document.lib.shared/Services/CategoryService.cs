﻿using document.lib.shared.Interfaces;
using document.lib.shared.TableEntities;

namespace document.lib.shared.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<DocLibCategory> GetCategoryByNameAsync(string name)
    {
        return await _categoryRepository.GetCategoryByNameAsync(name);
    }

    public async Task<DocLibCategory> CreateOrGetCategoryAsync(string category)
    {
        var docLibCategory = new DocLibCategory {Name = category};
        var categoryEntity = await _categoryRepository.GetCategoryByNameAsync(category);
        if (categoryEntity == null)
        {
            return await _categoryRepository.CreateCategoryAsync(docLibCategory);
        }

        return categoryEntity;
    }
}