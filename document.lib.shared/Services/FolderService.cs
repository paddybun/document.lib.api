using document.lib.shared.Interfaces;
using document.lib.shared.TableEntities;

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

    public async Task<DocLibFolder> GetFolderByNameAsync(string name)
    {
        return await _repository.GetFolderByNameAsync(name);
    }

    public async Task<DocLibFolder> GetActiveFolderAsync()
    {
        var folder = await _repository.GetActiveFolderAsync() ?? await CreateNewFolderAsync();
        return folder;
    }

    public async Task<DocLibFolder> CreateNewFolderAsync(int maxFolderSize = 200, int maxRegisterSize = 10)
    {
        return await _repository.CreateFolderAsync(new DocLibFolder { Name = $"New Folder {DateTimeOffset.UtcNow:yyyy-MM-dd}", DocumentsPerFolder = maxFolderSize, DocumentsPerRegister = maxRegisterSize});
    }

    public async Task<DocLibFolder> CreateOrGetFolderByIdAsync(string id, int maxFolderSize = 200, int maxRegisterSize = 10)
    {
        var folder = await _repository.GetFolderByIdAsync(id) ?? await CreateNewFolderAsync(maxFolderSize, maxRegisterSize);
        return folder;
    }

    public async Task<List<DocLibFolder>> GetAllAsync()
    {
        var folders = await _repository.GetAllFoldersAsync();
        return folders;
    }

    public async Task UpdateFolderAsync(DocLibFolder folder)
    {
        throw new NotImplementedException();
        // await _repository.UpdateFolderAsync(folder);
        // await _documentRepository.UpdateFolderReferenceAsync(folder.Id, folder.DisplayName);
    }

    public async Task AddDocumentToFolderAsync(DocLibFolder folder, DocLibDocument doc)
    {
        await _repository.AddDocumentToFolderAsync(folder, doc);
    }

    public async Task RemoveDocumentFromFolder(DocLibFolder folder, DocLibDocument doc)
    {
        await _repository.RemoveDocFromFolderAsync(folder, doc);
    }
}