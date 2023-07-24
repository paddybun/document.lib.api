using document.lib.shared.Constants;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.QueryDtos;
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
    
    public async Task<DocLibFolder> GetFolderAsync(FolderQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (!queryParameters.IsValid()) throw new InvalidQueryParameterException(queryParameters.GetType());
        DocLibFolder folder = null;
        if (queryParameters.Id.HasValue)
        {
            folder = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
                .Where(x => x.Id == queryParameters.Id.Value.ToString())
                .AsEnumerable()
                .FirstOrDefault();
        }
        else if (!string.IsNullOrWhiteSpace(queryParameters.Name))
        {
            folder = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
                .Where(x => x.Id == $"Folder.{queryParameters.Name}")
                .AsEnumerable()
                .FirstOrDefault();
        }
        else if (queryParameters.ActiveFolder.HasValue && queryParameters.ActiveFolder.Value)
        {
            folder = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
                .Where(x => x.IsFull == false)
                .AsEnumerable()
                .FirstOrDefault();
        }
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