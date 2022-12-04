﻿using document.lib.shared.TableEntities;

namespace document.lib.shared.Interfaces;

public interface ICategoryService
{
    Task<DocLibCategory> CreateOrGetCategoryAsync(string category);
}