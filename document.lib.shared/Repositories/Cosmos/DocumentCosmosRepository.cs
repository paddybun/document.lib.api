using document.lib.shared.Constants;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Models.Models;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace document.lib.shared.Repositories.Cosmos;

public class DocumentCosmosRepository : IDocumentRepository<DocLibDocument>
{
    private readonly Container _cosmosContainer;

    public DocumentCosmosRepository(IOptions<SharedConfig> config)
    {
        var cosmosClient = new CosmosClient(config.Value.CosmosDbConnection);
        var db = cosmosClient.GetDatabase(TableNames.Doclib);
        _cosmosContainer = db.GetContainer(TableNames.Doclib);
    }

    private static DocumentModel Map(DocLibDocument document)
    {
        return new DocumentModel
        {
            Id = document.Id,
            Name = document.Name,
            DisplayName = document.DisplayName,
            PhysicalName = document.PhysicalName,
            BlobLocation = document.BlobLocation,
            Company = document.Company,
            DateOfDocument = document.DateOfDocument,
            UploadDate = document.UploadDate,
            Description = document.Description,
            RegisterName = document.RegisterName,
            Tags = document.Tags.ToList(),
            Unsorted = document.Unsorted,
            CategoryId = "",
            CategoryName = document.CategoryName,
            Digital = document.DigitalOnly,
            FolderId = document.FolderId,
            FolderName = document.FolderName,
            DateModified = document.LastUpdate
        };
    }

    public Task<DocLibDocument> CreateDocumentAsync(DocumentModel document)
    {
        throw new NotImplementedException();
    }

    public Task<DocLibDocument?> GetDocumentAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<DocLibDocument?> GetDocumentAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<(int, List<DocLibDocument>)> GetDocumentsPagedAsync(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<(int, List<DocLibDocument>)> GetUnsortedDocumentsAsync(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetDocumentCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<(int, List<DocLibDocument>)> GetDocumentsForFolderAsync(string folderName, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<DocLibDocument> UpdateDocumentAsync(DocumentModel document, int? category = null, FolderModel? folder = null,
        TagModel[]? tags = null)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDocumentAsync(DocLibDocument doc)
    {
        throw new NotImplementedException();
    }
}