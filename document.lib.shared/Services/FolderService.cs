using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;

namespace document.lib.shared.Services;

public class FolderService(IFolderRepository folderRepository)
    : IFolderService
{
    public async Task<(int, FolderModel[])> GetFoldersPaged(int page, int pageSize)
    {
        return await folderRepository.GetFolders(page, pageSize);
    }

    public async Task<FolderModel?> GetFolderByNameAsync(string name)
    {
        var folder = await folderRepository.GetFolderAsync(new FolderModel{ Name = name});
        if (folder == null) return null;

        if (folder.CurrentRegister == null) AddNewRegister(folder);

        return folder;
    }

    public async Task<FolderModel?> GetFolderByIdAsync(string id)
    {
        var folder = await folderRepository.GetFolderAsync(new FolderModel { Id = id});
        if (folder == null) return null;

        if (folder.CurrentRegister == null) AddNewRegister(folder);
        return folder;
    }

    public async Task<FolderModel?> GetFolderByIdAsync(int id)
    {
        return await GetFolderByIdAsync(id.ToString());
    }

    public async Task<FolderModel> CreateNewFolderAsync(FolderModel folder)
    {
        return await folderRepository.CreateFolderAsync(folder);
    }

    public async Task<FolderModel> GetOrCreateActiveFolderAsync()
    {
        var model = new FolderModel { IsActive = true };
        return await folderRepository.GetFolderAsync(model) ??
               await SaveAsync(CreateDefaultFolderModel(), true);
    }

    public async Task<FolderModel> SaveAsync(FolderModel folderModel, bool createNew = false)
    {
        if (createNew)
        {
            return await folderRepository.CreateFolderAsync(folderModel);
        }

        return await folderRepository.UpdateFolderAsync(folderModel);
    }

    public async Task<FolderModel> GetOrCreateFolderByIdAsync(string id)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));

        var model = new FolderModel
        {
            Id = id,
            Name = id.Split('.').Last()
        };

        var folder = await folderRepository.GetFolderAsync(model) ??
                     await SaveAsync(CreateDefaultFolderModel(), true);

        return folder;
    }

    public async Task<List<FolderModel>> GetAllAsync()
    {
        var folders = await folderRepository.GetAllFoldersAsync();
        return folders;
    }

    public async Task<FolderModel?> UpdateFolderAsync(FolderModel folder)
    {
        return await folderRepository.UpdateFolderAsync(folder);
        
        // TODO: Reimplement update logic so it works for cosmos and sql
        // await _repository.UpdateFolderAsync(folder);
        // await _documentRepository.UpdateFolderReferenceAsync(folder.Id, folder.DisplayName);
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

    private FolderModel CreateDefaultFolderModel()
    {
        var name = $"New Folder {DateTimeOffset.UtcNow:yyyy-MM-dd}";
        return new FolderModel
        {
            Name = name,
            DisplayName = name,
            TotalDocuments = 0,
            DocumentsRegister = 10,
            DocumentsFolder = 100,
            IsFull = false
        };
    }

    private void AddNewRegister(FolderModel folder)
    {
        var lastRegisterNumber = folder.GetLastRegisterNumber();
        var newRegisterNumber = lastRegisterNumber + 1 ?? 1;

        var newRegister = new RegisterModel
        {
            Name = newRegisterNumber.ToString(),
            DisplayName = "New Register",
            DocumentCount = 0,
            Documents = []
        };

        folder.Registers.Add(newRegister);
        folder.CurrentRegister = newRegister;
    }
}