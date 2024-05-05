using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class DocumentSqlRepository(DocumentLibContext context) : IDocumentRepository<EfDocument>
{
    public async Task<EfDocument?> GetDocumentAsync(int id)
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

    public async Task<EfDocument?> GetDocumentAsync(string name)
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

    public async Task<(int, List<EfDocument>)> GetDocumentsPagedAsync(int page, int pageSize)
    {
        var count = await context.Documents.CountAsync();
        var efDocuments = await context.Documents
            .OrderBy(x => x.Id)
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (count, efDocuments);
    }

    public async Task<(int, List<EfDocument>)> GetUnsortedDocumentsAsync(int page, int pageSize)
    {
        var count = await context.Documents.Where(x => x.Unsorted).CountAsync();
        var efDocuments = await context
            .Documents
            .OrderBy(x => x.Id)
            .Where(x => x.Unsorted)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (count, efDocuments);
    }

    public async Task<(int, List<EfDocument>)> GetDocumentsForFolderAsync(string folderName, int page, int pageSize)
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
            .Where(x => x.Register.Folder!.Name == folderName)
            .OrderBy(x => x.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (folder?.TotalDocuments ?? 0, efDocuments);
    }

    public async Task<EfDocument> CreateDocumentAsync(DocumentModel document)
    {
        var registerName = document.Digital ? "digital" : document.RegisterName;
        var register = await context
            .Registers
            .SingleAsync(x => x.Name == registerName);
        var category = await context
            .Categories
            .SingleAsync(x => x.Name == document.CategoryName);
        var t = document.Tags?.Select(x => x) ?? [];
        var tags = await context
            .Tags
            .Where(x => t.Contains(x.Name))
            .ToListAsync();

        var assignments = tags.Select(x => new EfTagAssignment
        {
            Tag = x
        }).ToList();

        var efDocument = new EfDocument
        {
            Name = document.Name,
            DisplayName = document.DisplayName,
            BlobLocation = document.BlobLocation,
            Category = category,
            Company = document.Company,
            DateOfDocument = document.DateOfDocument,
            UploadDate = document.UploadDate,
            Description = document.Description,
            Digital = document.Digital,
            PhysicalName = document.PhysicalName,
            Register = register,
            Unsorted = document.Unsorted,
            Tags = assignments
        };

        await context.Documents.AddAsync(efDocument);
        await context.SaveChangesAsync();

        return efDocument;
    }

    public async Task<int> GetDocumentCountAsync()
    {
        return await context.Documents.CountAsync();
    }

    public async Task<EfDocument> UpdateDocumentAsync(DocumentModel document, int? category = null, FolderModel? folder = null,
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
            var efTagAssignments = efTags.Select(x => new EfTagAssignment
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

    public async Task DeleteDocumentAsync(EfDocument doc)
    {
        context.Documents.Remove(doc);
        await context.SaveChangesAsync();
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}