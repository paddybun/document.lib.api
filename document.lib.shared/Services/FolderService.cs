using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
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
        return await _repository.GetFolderAsync(new FolderQueryParameters(name: name));
    }

    public async Task<DocLibFolder> GetOrCreateActiveFolderAsync()
    {
        return await _repository.GetFolderAsync(new FolderQueryParameters(activeFolder: true)) ?? await CreateNewFolderAsync();
    }

    public async Task<DocLibFolder> CreateNewFolderAsync(int maxFolderSize = 200, int maxRegisterSize = 10)
    {
        return await _repository.CreateFolderAsync(new DocLibFolder { Name = $"New Folder {DateTimeOffset.UtcNow:yyyy-MM-dd}", DocumentsPerFolder = maxFolderSize, DocumentsPerRegister = maxRegisterSize});
    }

    public async Task<DocLibFolder> GetOrCreateFolderByIdAsync(string id, int maxFolderSize = 200, int maxRegisterSize = 10)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));

        DocLibFolder folder = null;
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