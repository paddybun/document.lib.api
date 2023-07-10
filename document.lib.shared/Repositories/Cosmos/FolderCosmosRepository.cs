using document.lib.shared.Constants;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Repositories.Cosmos;

public class FolderCosmosRepository : IFolderRepository
{
    private readonly Container _cosmosContainer;

    public FolderCosmosRepository(IOptions<AppConfiguration> config)
    {
        var cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        var db = cosmosClient.GetDatabase(TableNames.Doclib);
        _cosmosContainer = db.GetContainer(TableNames.Doclib);
    }

    public async Task<DocLibFolder> GetFolderByNameAsync(string folderName)
    {
        return await GetFolderByIdAsync($"Folder.{folderName}");
    }

    public async Task<DocLibFolder> GetFolderByIdAsync(string id)
    {
        var folder = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
            .Where(x => x.Id == id)
            .AsEnumerable()
            .FirstOrDefault();
        return await Task.FromResult(folder);
    }

    public async Task<DocLibFolder> GetActiveFolderAsync()
    {
        var folder = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
            .Where(x => x.IsFull == false)
            .AsEnumerable()
            .FirstOrDefault();
        return await Task.FromResult(folder);
    }

    public async Task<List<DocLibFolder>> GetAllFoldersAsync()
    {
        var folders = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
            .Where(x => x.Id.StartsWith("Folder."))
            .AsEnumerable();
        return await Task.FromResult(folders.ToList());
    }

    public async Task<DocLibFolder> CreateFolderAsync(DocLibFolder folder)
    {
        await _cosmosContainer.UpsertItemAsync(folder, new PartitionKey(folder.Id));
        return folder;
    }

    public async Task UpdateFolderAsync(DocLibFolder folder)
    {
        await _cosmosContainer.UpsertItemAsync(folder, new PartitionKey(folder.Id));
    }

    public Task AddDocumentToFolderAsync(DocLibFolder folder, DocLibDocument document)
    {
        throw new NotImplementedException();
    }

    public Task RemoveDocFromFolderAsync(DocLibFolder folder, DocLibDocument document)
    {
        throw new NotImplementedException();
    }
}