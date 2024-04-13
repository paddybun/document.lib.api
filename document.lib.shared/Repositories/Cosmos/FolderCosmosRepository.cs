using document.lib.shared.Constants;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.Models;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
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
    
    public async Task<FolderModel> GetFolderAsync(FolderQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (!queryParameters.IsValid()) throw new InvalidQueryParameterException(queryParameters.GetType());
        IAsyncEnumerable<DocLibFolder> folders = null;
        if (queryParameters.Id.HasValue)
        {
            folders = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
                .Where(x => x.Id == queryParameters.Id.Value.ToString())
                .AsAsyncEnumerable();
        }
        else if (!string.IsNullOrWhiteSpace(queryParameters.Name))
        {
            folders = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
                .Where(x => x.Name == queryParameters.Name)
                .AsAsyncEnumerable();
        }
        else if (queryParameters.ActiveFolder.HasValue && queryParameters.ActiveFolder.Value)
        {
            folders = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
                .Where(x => x.IsFull == false)
                .AsAsyncEnumerable();
        }
        
        var tmpFolders = new List<FolderModel>();
        await foreach (var folder in folders)
        {
            tmpFolders.Add(MapToModel(folder));
        }

        return tmpFolders.FirstOrDefault();
    }

    public async Task<List<FolderModel>> GetAllFoldersAsync()
    {
        var folders = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
            .Where(x => x.Id.StartsWith("Folder."))
            .AsEnumerable();

        var result = folders.Select(MapToModel).ToList();
        return await Task.FromResult(result);
    }

    public async Task<FolderModel> CreateFolderAsync(FolderModel folder)
    {
        var folderEntity = MapToEntity(folder);
        await _cosmosContainer.UpsertItemAsync(folderEntity, new PartitionKey(folder.Id));
        var newFolderEntity = await GetFolderAsync(new FolderQueryParameters(name: folder.Name));
        return newFolderEntity;
    }

    public async Task<FolderModel> UpdateFolderAsync(FolderModel folder)
    {
        await _cosmosContainer.UpsertItemAsync(folder, new PartitionKey(folder.Id));
        var updatedFolderEntity = await GetFolderAsync(new FolderQueryParameters(name: folder.Name));
        return updatedFolderEntity;
    }

    public Task AddDocumentToFolderAsync(FolderModel folder, DocumentModel document)
    {
        throw new NotImplementedException();
    }

    public Task RemoveDocFromFolderAsync(FolderModel folder, DocumentModel document)
    {
        throw new NotImplementedException();
    }

    private static DocLibFolder MapToEntity(FolderModel folderModel)
    {
        return new DocLibFolder
        {
            Id = folderModel.Id,
            Name = folderModel.Name,
            DisplayName = folderModel.DisplayName,
            CurrentRegister = folderModel.CurrentRegisterName,
            Registers = folderModel.Registers.ToDictionary(x => x.Name, y => y.DocumentCount),
            TotalDocuments = folderModel.TotalDocuments,
            DocumentsPerRegister = folderModel.DocumentsRegister,
            DocumentsPerFolder = folderModel.DocumentsFolder,
            CreatedAt = folderModel.CreatedAt,
            IsFull = folderModel.IsFull
        };
    }

    private static FolderModel MapToModel(DocLibFolder doclibFolder)
    {
        if (doclibFolder == null) return null;
        return new FolderModel
        {
            Name = doclibFolder.Name,
            DisplayName = doclibFolder.DisplayName,
            CreatedAt = doclibFolder.CreatedAt,
            CurrentRegisterName = doclibFolder.CurrentRegister,
            DocumentsFolder = doclibFolder.DocumentsPerFolder,
            DocumentsRegister = doclibFolder.DocumentsPerRegister,
            Id = doclibFolder.Id,
            IsFull = doclibFolder.IsFull,
            Registers = doclibFolder.Registers.Select(x => MapToModel(x, doclibFolder.Id, doclibFolder.Name)).ToList()
        };
    }

    private static RegisterModel MapToModel(KeyValuePair<string,int> registerDictEntry, string folderId, string folderName)
    {
        var (registerKey, registerValue) = registerDictEntry;
        return new RegisterModel
        {
            Id = "",
            Name = registerKey,
            DisplayName = registerKey,
            Documents = null,
            FolderId = folderId,
            FolderName = folderName,
            DocumentCount = registerValue
        };
    }
}