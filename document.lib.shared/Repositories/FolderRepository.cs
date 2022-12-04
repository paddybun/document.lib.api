using document.lib.shared.Constants;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Repositories;

public class FolderRepository : IFolderRepository
{
    private readonly Container _cosmosContainer;

    public FolderRepository(IOptions<AppConfiguration> config)
    {
        var cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        var db = cosmosClient.GetDatabase(TableNames.Doclib);
        _cosmosContainer = db.GetContainer(TableNames.Doclib);
    }

    public DocLibFolder GetFolderByName(string folderName)
    {
        return GetFolderById($"Folder.{folderName}");
    }

    public DocLibFolder GetFolderById(string id)
    {
        var folder = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
            .Where(x => x.Id == id)
            .AsEnumerable()
            .FirstOrDefault();
        return folder;
    }

    public DocLibFolder GetCurrentlyActiveFolder()
    {
        var folder = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
            .Where(x => x.IsFull == false)
            .AsEnumerable()
            .FirstOrDefault();
        return folder;
    }

    public List<DocLibFolder> GetAllFolders()
    {
        var folders = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
            .Where(x => x.Id.StartsWith("Folder."))
            .AsEnumerable();
        return folders.ToList();
    }

    public async Task UpdateNameAsync(DocLibFolder folder)
    {
        await _cosmosContainer.UpsertItemAsync(folder, new PartitionKey(folder.Id));
    }
}