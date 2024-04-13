using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using document.lib.shared.Models.QueryDtos;

namespace document.lib.shared.Services;

public class FolderService(IFolderRepository folderRepository)
    : IFolderService
{
    public async Task<FolderModel?> GetFolderByNameAsync(string name)
    {
        var folder = await folderRepository.GetFolderAsync(new FolderModel{ Name = name});
        if (folder == null) return null;

        if (folder.CurrentRegister == null)
        {
            var lastRegisterNumber = folder.GetLastRegisterNumber();
            var newRegisterNumber = lastRegisterNumber + 1 ?? 1;

            var newRegister = new RegisterModel
            {
                Name = newRegisterNumber.ToString(),
                DisplayName = "New Register",
                DocumentCount = 0,
                Documents = new List<DocumentModel>()
            };
        }
    }

    public async Task<FolderModel> GetOrCreateActiveFolderAsync()
    {
        return await folderRepository.GetFolderAsync(new FolderQueryParameters(activeFolder: true)) ??
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

        FolderModel folder;
        if (int.TryParse(id, out var parsedId))
        {
            folder = await folderRepository.GetFolderAsync(new FolderQueryParameters(id: parsedId)) ??
                     await SaveAsync(CreateDefaultFolderModel(), true);
        }
        else
        {
            // If id could not be parsed as int, it is assumed that a cosmos id is used in the format: 'Folder.f9c900c2-aaa4-41c9-a7d2-d4ad928ffc95'
            var cosmosId = id.Split('.').Last();
            folder = await folderRepository.GetFolderAsync(new FolderQueryParameters(name: cosmosId)) ??
                     await SaveAsync(CreateDefaultFolderModel(), true);
        }
        return folder;
    }

    public async Task<List<FolderModel>> GetAllAsync()
    {
        var folders = await folderRepository.GetAllFoldersAsync();
        return folders;
    }

    public Task<FolderModel> UpdateFolderAsync(FolderModel folder)
    {
        throw new NotImplementedException();
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
}