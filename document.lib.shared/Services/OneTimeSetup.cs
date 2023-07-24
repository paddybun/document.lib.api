using document.lib.shared.Interfaces;

namespace document.lib.shared.Services;

public class OneTimeSetup
{
    private readonly ICategoryService _categoryService;
    private readonly IFolderService _folderService;

    public OneTimeSetup(ICategoryService categoryService, IFolderService folderService)
    {
        _categoryService = categoryService;
        _folderService = folderService;
    }

    public async Task CreateDefaultsAsync()
    {
        await _categoryService.CreateOrGetCategoryAsync("uncategorized");
        await _folderService.GetOrCreateFolderByIdAsync("unsorted", int.MaxValue, int.MaxValue);
        await _folderService.GetOrCreateFolderByIdAsync("digital", int.MaxValue, int.MaxValue);
    }
}