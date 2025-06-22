using document.lib.data.entities;
using document.lib.ef;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class DocumentSqlRepository(DocumentLibContext context) : IDocumentRepository<Document>
{
    public async Task<Document?> GetDocumentAsync(int id)
    {
        var doc = await context
            .Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .ThenInclude(x => x.Tag)
            .SingleOrDefaultAsync(x => x.Id== id);
        return doc;
    }

    public async Task<Document?> GetDocumentAsync(string name)
    {
        var doc = await context.
            Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)!
            .ThenInclude(x => x.Tag)
            .SingleOrDefaultAsync(x => x.Name == name);
        return doc;
    }

    public async Task<PagedResult<Document>> GetDocumentsPagedAsync(int page, int pageSize)
    {
        var count = await context.Documents.CountAsync();
        var efDocuments = await context.Documents
            .OrderBy(x => x.Id)
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedResult<Document>(efDocuments, count);
    }

    public async Task<PagedResult<Document>> GetUnsortedDocumentsAsync(int page, int pageSize)
    {
        var count = await context.Documents.Where(x => x.Unsorted).CountAsync();
        var efDocuments = await context
            .Documents
            .OrderBy(x => x.Id)
            .Where(x => x.Unsorted)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Document>(efDocuments, count);
    }

    public async Task<PagedResult<Document>> GetDocumentsForFolderAsync(string folderName, int page, int pageSize)
    {
        var folder = await context
            .Folders
            .Where(x => x.Name == folderName)
            .SingleOrDefaultAsync();
        var efDocuments = await context
            .Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .ThenInclude(x => x.Tag)
            .Where(x => x.Register.Folder!.Name == folderName)
            .OrderBy(x => x.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedResult<Document>(efDocuments, folder?.TotalDocuments ?? 0);
    }

    public async Task<Document> CreateDocumentAsync(Document document)
    {
        await context.Documents.AddAsync(document);
        await context.SaveChangesAsync();
        return document;
    }

    public async Task<int> GetDocumentCountAsync()
    {
        return await context.Documents.CountAsync();
    }

    public async Task<Document> UpdateDocumentAsync(DocumentModel document, int? category = null, FolderModel? folder = null,
        TagModel[]? tags = null)
    {
        var efDoc = context
            .Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .Single();

        if (category != null)
        {
            var efCategory = await context.Categories.SingleAsync(x => x.Id == category);
            efDoc.Category = efCategory;
        }
        if (tags != null)
        {
            var efTags = await context.Tags.Where(x => tags.Select(y => (int)y.Id!).Contains(x.Id)).ToListAsync();
            var efTagAssignments = efTags.Select(x => new TagAssignment
            {
                Tag = x
            }).ToList();
            efDoc.Tags = efTagAssignments;
        }
        if (folder != null)
        {
            var id = (int?)folder.Id;
            var efFolder = await context
                .Folders
                .Include(x => x.Registers)
                .SingleAsync(x => x.Id == id);

            if (efFolder.CurrentRegister != null)
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

        context.Update(efDoc);
        await context.SaveChangesAsync();
        return efDoc;
    }

    public async Task DeleteDocumentAsync(Document doc)
    {
        context.Documents.Remove(doc);
        await context.SaveChangesAsync();
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}