using document.lib.shared.Constants;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.Models;
using document.lib.shared.Models.Update;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Repositories.Cosmos;

public class FolderCosmosRepository : IFolderRepository<DocLibFolder>
{
    private readonly Container _cosmosContainer;
    
    public FolderCosmosRepository(IOptions<SharedConfig> config)
    {
        var cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        var db = cosmosClient.GetDatabase(TableNames.Doclib);
        _cosmosContainer = db.GetContainer(TableNames.Doclib);
    }

    public Task<DocLibFolder?> GetFolderAsync(int id)
    {
        throw new NotImplementedException("Get folder by int id not supported. Use GetFolderAsync(string name) instead.");
    }

    public async Task<DocLibFolder?> GetFolderAsync(string name)
    {
        var folder = await _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
            .Where(x => x.Name == name)
            .ToListAsync();
        return folder.FirstOrDefault();
    }

    public async Task<DocLibFolder?> GetActiveFolderAsync()
    {
        var folders = await _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
            .Where(x => x.IsFull == false)
            .ToListAsync();
        return folders.FirstOrDefault();
    }

    public async Task<List<DocLibFolder>> GetAllFoldersAsync()
    {
        var folders = _cosmosContainer.GetItemLinqQueryable<DocLibFolder>(true)
            .Where(x => x.Id.StartsWith("Folder."))
            .AsEnumerable();
        return await Task.FromResult(folders.ToList());
    }
    
    public async Task<DocLibFolder> CreateFolderAsync(string name, int docsPerRegister = 10, int docsPerFolder = 150, string? displayName = null)
    {
        var folder = new DocLibFolder
        {
            Id = name,
            Name = name,
            DisplayName = displayName,
            CurrentRegister = "1",
            Registers = new Dictionary<string, int>{{"1", 0}},
            TotalDocuments = 0,
            DocumentsPerRegister = docsPerRegister,
            DocumentsPerFolder = docsPerFolder,
            CreatedAt = DateTimeOffset.Now,
            IsFull = false
        }; 
        var key = folder.Id;
        await _cosmosContainer.UpsertItemAsync(folder, new PartitionKey(key));
        var newFolder = await GetFolderAsync(name);
        return newFolder!;
    }

    public async Task<DocLibFolder?> UpdateFolderAsync(FolderUpdateModel updateModel, string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        var folder = await GetFolderAsync(name!);
        if (folder == null) return null;
        
        folder.DisplayName = updateModel.DisplayName;
        folder.DocumentsPerFolder = updateModel.DocsPerFolder;
        folder.DocumentsPerRegister = updateModel.DocsPerRegister;
        folder.IsFull = updateModel.IsFull;
        folder.TotalDocuments = updateModel.TotalDocuments;
        
        await _cosmosContainer.UpsertItemAsync(folder, new PartitionKey(name));
        var updatedFolderEntity = await GetFolderAsync(name);
        return updatedFolderEntity!;
    }

    public Task<DocLibFolder?> AddDocumentToFolderAsync(FolderModel folder, DocumentModel document)
    {
        throw new NotImplementedException();
    }

    public Task RemoveDocFromFolderAsync(FolderModel folder, DocumentModel document)
    {
        throw new NotImplementedException();
    }

    public Task<(int, List<DocLibFolder>)> GetFolders(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync()
    {
        throw new NotImplementedException();
    }
}