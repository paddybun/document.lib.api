using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;

namespace document.lib.shared.Services;

public class FolderService : IFolderService
{
    private readonly IFolderRepository _repository;
    private readonly IDocumentRepository _documentRepository;

    public FolderService(IFolderRepository repository, IDocumentRepository documentRepository)
    {
        _repository = repository;
        _documentRepository = documentRepository;
    }

    public async Task<FolderModel> GetFolderByNameAsync(string name)
    {
        return await _repository.GetFolderAsync(new FolderQueryParameters(name: name));
    }

    public async Task<FolderModel> GetOrCreateActiveFolderAsync()
    {
        return await _repository.GetFolderAsync(new FolderQueryParameters(activeFolder: true)) ?? await CreateNewFolderAsync();
    }

    public async Task<FolderModel> CreateNewFolderAsync(int maxFolderSize = 200, int maxRegisterSize = 10)
    {
        return await _repository.CreateFolderAsync(new FolderModel { Name = $"New Folder {DateTimeOffset.UtcNow:yyyy-MM-dd}", DocumentsFolder = maxFolderSize, DocumentsRegister = maxRegisterSize});
    }

    public async Task<FolderModel> GetOrCreateFolderByIdAsync(string id, int maxFolderSize = 200, int maxRegisterSize = 10)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));

        FolderModel folder;
        if (int.TryParse(id, out var parsedId))
        {
            folder = await _repository.GetFolderAsync(new FolderQueryParameters(id: parsedId)) ?? await CreateNewFolderAsync(maxFolderSize, maxRegisterSize);
        }
        else
        {
            // If id could not be parsed as int, it is assumed that a cosmos id is used in the format: 'Folder.f9c900c2-aaa4-41c9-a7d2-d4ad928ffc95'
            var cosmosId = id.Split('.').Last();
            folder = await _repository.GetFolderAsync(new FolderQueryParameters(name: cosmosId)) ?? await CreateNewFolderAsync(maxFolderSize, maxRegisterSize);
        }
        return folder;
    }

    public async Task<List<FolderModel>> GetAllAsync()
    {
        var folders = await _repository.GetAllFoldersAsync();
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
}