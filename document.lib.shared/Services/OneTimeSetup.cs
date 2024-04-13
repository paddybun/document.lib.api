using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;

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
        await _categoryService.SaveAsync(new CategoryModel
        {
            Name = "uncategorized",
            DisplayName = "uncategorized",
            Description = "Category for uncategorized documents",
            
        }, true);

        await _folderService.SaveAsync(new FolderModel
        {
            Name = "unsorted",
            DisplayName = "unsorted",
            DocumentsRegister = int.MaxValue,
            DocumentsFolder = int.MaxValue,
            IsFull = false,
        }, true);
        
        await _folderService.SaveAsync(new FolderModel
        {
            Name = "digital",
            DisplayName = "digital",
            DocumentsRegister = int.MaxValue,
            DocumentsFolder = int.MaxValue,
            IsFull = false,
        }, true);
    }
}