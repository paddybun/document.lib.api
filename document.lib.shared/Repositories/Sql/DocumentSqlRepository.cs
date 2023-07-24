using Azure;
using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.TableEntities;
using Microsoft.EntityFrameworkCore;
using DocLibDocument = document.lib.shared.TableEntities.DocLibDocument;

namespace document.lib.shared.Repositories.Sql;

public class DocumentSqlRepository: IDocumentRepository
{
    private readonly DocumentLibContext _context;

    public DocumentSqlRepository(DocumentLibContext context)
    {
        _context = context;
    }

    public async Task<DocLibDocument> CreateDocumentAsync(DocLibDocument document)
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
            Digital = document.DigitalOnly,
            PhysicalName = document.PhysicalName,
            Register = register,
            Unsorted = true
        };
        return Map(efDocument);
    }

    public async Task<DocLibDocument> GetDocumentById(string id)
    {
        var efDocument = await _context
            .Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .SingleOrDefaultAsync(x => x.Id == int.Parse(id));
        return Map(efDocument);
    }

    public async Task<DocLibDocument> GetDocumentByName(string name)
    {
        var efDocument = await _context
            .Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .SingleOrDefaultAsync(x => x.Name == name);

        return Map(efDocument);
    }

    public async Task<List<DocLibDocument>> GetDocuments(int page, int count)
    {
        var efDocuments = await _context
            .Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .Skip(page).Take(count)
            .ToListAsync();

        var mapped = efDocuments.Select(Map).ToList();
        return mapped;
    }

    public async Task<int> GetDocumentCount()
    {
        return await _context.Documents.CountAsync();
    }

    public async Task<List<DocLibDocument>> GetDocumentsForFolder(string folderName, int page, int count)
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

    public async Task<DocLibDocument> UpdateDocumentAsync(DocLibDocument document, DocLibCategory category = null, DocLibFolder folder = null, DocLibTag[] tags = null)
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
        efDoc.Digital = document.DigitalOnly;

        _context.Update(efDoc);
        await _context.SaveChangesAsync();
        return Map(efDoc);
    }

    public async Task DeleteDocumentAsync(DocLibDocument doc)
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

    private DocLibDocument Map(EfDocument efDocument)
    {
        if (efDocument == null) return null;

        return new DocLibDocument
        {
            Id = efDocument.Id.ToString(),
            Name = efDocument.Name,
            DisplayName = efDocument.DisplayName,
            Description = efDocument.Description,
            Category = efDocument.Category?.DisplayName,
            BlobLocation = efDocument.BlobLocation,
            FolderName = efDocument.Register.Folder.Name,
            Company = efDocument.Company,
            DateOfDocument = efDocument.DateOfDocument,
            LastUpdate = efDocument.DateModified,
            UploadDate = efDocument.DateCreated,
            DigitalOnly = efDocument.Digital,
            PhysicalName = efDocument.PhysicalName,
            Tags = efDocument.Tags?.Select(x => x.Name).ToArray(),
            RegisterName = efDocument.Register.Name,
            FolderId = efDocument.Register.Folder.Id.ToString(),
            Unsorted = efDocument.Unsorted
        };
    }
}