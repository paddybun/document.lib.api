using document.lib.shared.Interfaces;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;

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

    public DocLibFolder GetActiveFolder()
    {
        return _repository.GetCurrentlyActiveFolder();
    }

    public List<DocLibFolder> GetAll()
    {
        var folders = _repository.GetAllFolders();
        return folders;
    }

    public async Task UpdateFolderAsync(DocLibFolder folder)
    {
        await _repository.UpdateNameAsync(folder);
        await _documentRepository.UpdateFolderReferenceAsync(folder.Id, folder.DisplayName);
    }
}