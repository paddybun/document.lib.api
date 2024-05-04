using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using document.lib.shared.Repositories.Models;

namespace document.lib.shared.Services;

public class FolderSqlService(IFolderRepository<EfFolder> folderRepository)
    : IFolderService
{
    public async Task<(int, List<FolderModel>)> GetFoldersPaged(int page, int pageSize)
    {
        var (count, folders) = await folderRepository.GetFolders(page, pageSize);
        var mapped = folders.Select(MapShallow).ToList();
        return (count, mapped);
    }

    public async Task<FolderModel?> GetFolderByNameAsync(string name)
    {
        var folder = await folderRepository.GetFolderAsync(name);
        if (folder == null) return null;

        return Map(folder);
    }

    public async Task<FolderModel?> GetFolderByIdAsync(int id)
    {
        var folder = await folderRepository.GetFolderAsync(id);
        if (folder == null) return null;

        return Map(folder);
    }

    public async Task<FolderModel> CreateNewFolderAsync(int docsPerFolder = 150, int docsPerRegister = 10, string? displayName = null)
    {
        var name = Guid.NewGuid();
        var folder = await folderRepository.CreateFolderAsync(name.ToString(), docsPerRegister, docsPerFolder, displayName);
        return Map(folder);
    }

    public async Task<FolderModel> GetOrCreateActiveFolderAsync()
    {
        var folder = await folderRepository.GetActiveFolderAsync();
        if (folder == null)
        {
            var name = Guid.NewGuid().ToString();
            folder = await folderRepository.CreateFolderAsync(name);
        }
        return Map(folder);
    }
    
    public async Task<List<FolderModel>> GetAllAsync()
    {
        var folders = await folderRepository.GetAllFoldersAsync();
        return folders.Select(Map).ToList();
    }

    public async Task<FolderModel?> UpdateFolderAsync(FolderModel folder)
    {
        folder.IsFull = folder.TotalDocuments >= folder.DocumentsFolder;
        var updateModel = FolderUpdateModel.CreateFromModel(folder);
        
        var updatedFolder = await folderRepository.UpdateFolderAsync(updateModel);
        return updatedFolder == null ? null : Map(updatedFolder);
    }

    public async Task AddDocumentToFolderAsync(FolderModel folder, DocumentModel doc)
    {
        throw new NotImplementedException();
        // TODO: Reimplement update logic so it works for cosmos and sql, move repo logic to here
        // await _repository.AddDocumentToFolderAsync(folder, doc);
    }

    public async Task RemoveDocumentFromFolder(FolderModel folder, DocumentModel doc)
    {
        throw new NotImplementedException();
        // TODO: Reimplement removal logic so it works for cosmos and sql, move repo logic to here
        // await _repository.RemoveDocFromFolderAsync(folder, doc);
    }
    
    private static FolderModel MapShallow(EfFolder efFolder)
    {
        return new FolderModel
        {
            Id = efFolder.Id.ToString(),
            Name = efFolder.Name,
            DisplayName = efFolder.DisplayName,
            CreatedAt = efFolder.DateCreated,
            IsActive = false,
            DocumentsFolder = efFolder.MaxDocumentsFolder,
            DocumentsRegister = efFolder.MaxDocumentsRegister,
            IsFull = efFolder.IsFull,
            TotalDocuments = efFolder.TotalDocuments
        };
    }

    private static FolderModel Map(EfFolder efFolder)
    {
        return new FolderModel
        {
            Name = efFolder.Name,
            DisplayName = efFolder.DisplayName,
            CreatedAt = efFolder.DateCreated,
            IsActive = false,
            CurrentRegisterName = efFolder.CurrentRegister?.Name ?? string.Empty,
            DocumentsFolder = efFolder.MaxDocumentsFolder,
            DocumentsRegister = efFolder.MaxDocumentsRegister,
            Id = efFolder.Id.ToString(),
            IsFull = efFolder.IsFull,
            Registers = efFolder
                .Registers?.Select(x => Map(x, efFolder))
                .ToList() ?? [],
            TotalDocuments = efFolder.TotalDocuments,
            CurrentRegister = Map(efFolder.CurrentRegister!, efFolder)
        };
    }

    private static RegisterModel Map(EfRegister efRegister, EfFolder efFolder)
    {
        return new RegisterModel
        {
            Name = efRegister.Name,
            DisplayName = efRegister.DisplayName,
            DocumentCount = efRegister.DocumentCount,
            Id = efRegister.Id.ToString(),
            FolderId = efFolder.Id.ToString(),
            FolderName = efFolder.Name
        };
    }
}