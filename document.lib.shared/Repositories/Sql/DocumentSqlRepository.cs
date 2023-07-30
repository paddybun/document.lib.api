using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public class DocumentSqlRepository: IDocumentRepository
{
    private readonly DocumentLibContext _context;

    public DocumentSqlRepository(DocumentLibContext context)
    {
        _context = context;
    }

    public async Task<DocumentModel> CreateDocumentAsync(DocumentModel document)
    {
        var register = await _context.Registers.SingleOrDefaultAsync(x => x.Name == "unsorted");
        var efDocument = new EfDocument
        {
            Name = document.Name,
            DisplayName = document.DisplayName,
            BlobLocation = document.BlobLocation,
            Category = _context.Categories.Single(x => x.Name == "uncategorized"),
            Company = document.Company,
            DateOfDocument = document.DateOfDocument,
            UploadDate = DateTimeOffset.UtcNow,
            Description = document.Description,
            Digital = document.Digital,
            PhysicalName = document.PhysicalName,
            Register = register,
            Unsorted = true
        };
        return Map(efDocument);
    }

    public async Task<DocumentModel> GetDocumentAsync(DocumentQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (!queryParameters.IsValid()) throw new InvalidQueryParameterException(queryParameters.GetType());

        EfDocument efDocument;
        if (queryParameters.Id.HasValue)
        {
            efDocument = await _context
                .Documents
                .Include(x => x.Register)
                .ThenInclude(x => x.Folder)
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Id == queryParameters.Id.Value);
        }
        else
        {
            efDocument = await _context
                .Documents
                .Include(x => x.Register)
                .ThenInclude(x => x.Folder)
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Name == queryParameters.Name);
        }
        return Map(efDocument);
    }

    public async Task<List<DocumentModel>> GetDocumentsAsync(int page, int count)
    {
        var efDocuments = await _context
            .Documents
            .Include(x => x.Tags)
            .Skip(page).Take(count)
            .ToListAsync();

        var mapped = efDocuments.Select(Map).ToList();
        return mapped;
    }

    public async Task<int> GetDocumentCountAsync()
    {
        return await _context.Documents.CountAsync();
    }

    public async Task<List<DocumentModel>> GetDocumentsForFolderAsync(string folderName, int page, int count)
    {
        var efDocuments = await _context
            .Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .Where(x => x.Register.Folder.Name == folderName)
            .Skip(page).Take(count)
            .ToListAsync();

        var mapped = efDocuments.Select(Map).ToList();
        return mapped;
    }

    public async Task<DocumentModel> UpdateDocumentAsync(DocumentModel document, CategoryModel category = null, FolderModel folder = null,
        TagModel[] tags = null)
    {
        var efDoc = _context
            .Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .Single();

        if (category != null)
        {
            var efCategory = await _context.Categories.SingleAsync(x => x.Id == int.Parse(category.Id));
            efDoc.Category = efCategory;
        }
        if (tags != null)
        {
            var efTags = await _context.Tags.Where(x => tags.Select(x => int.Parse(x.Id)).Contains(x.Id)).ToArrayAsync();
            efDoc.Tags = efTags;
        }
        if (folder != null)
        {
            var efFolder = await _context
                .Folders
                .Include(x => x.CurrentRegister)
                .SingleAsync(x => x.Id == int.Parse(category.Id));
            efDoc.Register = efFolder.CurrentRegister;
        }

        efDoc.Name = document.Name;
        efDoc.DisplayName = document.DisplayName;
        efDoc.Description = document.Description;
        efDoc.Company = document.Company;
        efDoc.Unsorted = document.Unsorted;
        efDoc.BlobLocation = document.BlobLocation;
        efDoc.DateOfDocument = document.DateOfDocument;
        efDoc.Digital = document.Digital;

        _context.Update(efDoc);
        await _context.SaveChangesAsync();
        return Map(efDoc);
    }

    public async Task DeleteDocumentAsync(DocumentModel doc)
    {
        var docId = int.Parse(doc.Id);
        var efDoc = await _context.Documents.SingleAsync(x => x.Id == docId);
        _context.Remove(efDoc);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(string documentId)
    {
        var docId = int.Parse(documentId);
        var efDoc = await _context.Documents.SingleAsync(x => x.Id == docId);
        _context.Remove(efDoc);
        await _context.SaveChangesAsync();
    }

    private DocumentModel Map(EfDocument efDocument)
    {
        if (efDocument == null) return null;

        return new DocumentModel
        {
            Id = efDocument.Id.ToString(),
            Name = efDocument.Name,
            DisplayName = efDocument.DisplayName,
            Description = efDocument.Description,
            CategoryName = efDocument.Category?.DisplayName,
            BlobLocation = efDocument.BlobLocation,
            FolderName = efDocument.Register.Folder.Name,
            Company = efDocument.Company,
            DateOfDocument = efDocument.DateOfDocument,
            DateModified = efDocument.DateModified,
            UploadDate = efDocument.DateCreated,
            Digital = efDocument.Digital,
            PhysicalName = efDocument.PhysicalName,
            Tags = efDocument.Tags?.Select(x => x.Name).ToList(),
            RegisterName = efDocument.Register.Name,
            FolderId = efDocument.Register.Folder.Id.ToString(),
            Unsorted = efDocument.Unsorted
        };
    }
}