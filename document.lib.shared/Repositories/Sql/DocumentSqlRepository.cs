using System.Linq.Expressions;
using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.ef.Helpers;
using document.lib.shared.Helper;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class DocumentSqlRepository(DocumentLibContext context) : IDocumentRepository
{
    public async Task<DocumentModel> CreateDocumentAsync(DocumentModel document)
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

        return Map(efDocument);
    }

    public async Task<DocumentModel?> GetDocumentAsync(DocumentModel model)
    {
        Expression<Func<EfDocument, bool>> getDocumentExpression;
        
        if (PropertyChecker.Values.Any(model, x => x.Id)) 
            getDocumentExpression = x => x.Id == int.Parse(model.Id);
        else if (PropertyChecker.Values.Any(model, x => x.Name)) 
            getDocumentExpression = x => x.Name == model.Name;
        else { return null; }

        var efDocument = await context
            .Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)!
            .ThenInclude(x => x.Tag)
            .SingleOrDefaultAsync(getDocumentExpression);
        
        return efDocument == null ? null : Map(efDocument);
    }

    public async Task<List<DocumentModel>> GetAllDocumentsAsync()
    {
        var efDocuments = await context
            .Documents
            .Include(x => x.Tags)
            .ToListAsync();
        return efDocuments.Select(Map).ToList();
    }

    public async Task<(int, List<DocumentModel>)> GetDocumentsPagedAsync(int page, int pageSize)
    {
        var count = await context.Documents.CountAsync();
        var efDocuments = await context.Documents
            .OrderBy(x => x.Id)
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var documents = efDocuments.Select(Map).ToList();
        return (count, documents);
    }

    public async Task<List<DocumentModel>> GetUnsortedDocumentsAsync()
    {
        var efDocuments = await context
            .Documents
            .Where(x => x.Unsorted)
            .OrderBy(x => x.Id)
            .ToListAsync();
        return efDocuments.Select(Map).ToList();
    }

    public async Task<int> GetDocumentCountAsync()
    {
        return await context.Documents.CountAsync();
    }

    public async Task<List<DocumentModel>> GetDocumentsForFolderAsync(string folderName, int page, int count)
    {
        var efDocuments = await context
            .Documents
            .Include(x => x.Register)
            .ThenInclude(x => x.Folder)
            .Include(x => x.Category)
            .Include(x => x.Tags)
            .Where(x => x.Register.Folder!.Name == folderName)
            .Skip(page).Take(count)
            .ToListAsync();

        var mapped = efDocuments.Select(Map).ToList();
        return mapped;
    }

    public async Task<DocumentModel> UpdateDocumentAsync(DocumentModel document, CategoryModel? category = null, FolderModel? folder = null,
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
            var efCategory = await context.Categories.SingleAsync(x => x.Id == int.Parse(category.Id));
            efDoc.Category = efCategory;
        }
        if (tags != null)
        {
            var efTags = await context.Tags.Where(x => tags.Select(x => int.Parse(x.Id)).Contains(x.Id)).ToListAsync();
            var efTagAssignments = efTags.Select(x => new EfTagAssignment
            {
                Tag = x
            }).ToList();
            efDoc.Tags = efTagAssignments;
        }
        if (folder != null)
        {
            int.TryParse(folder.Id, out var folderId);
            var efFolder = await context
                .Folders
                .Include(x => x.Registers)
                .SingleAsync(x => x.Id == folderId);

            FolderEntityHelpers.AssignCurrentRegister(efFolder);

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
        return Map(efDoc);
    }

    public async Task DeleteDocumentAsync(DocumentModel doc)
    {
        var docId = int.Parse(doc.Id);
        var efDoc = await context.Documents.SingleAsync(x => x.Id == docId);
        context.Remove(efDoc);
        await context.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(string documentId)
    {
        var docId = int.Parse(documentId);
        var efDoc = await context.Documents.SingleAsync(x => x.Id == docId);
        context.Remove(efDoc);
        await context.SaveChangesAsync();
    }

    private DocumentModel Map(EfDocument efDocument)
    {
        var tags = efDocument.Tags?.Select(x => x.Tag?.Name ?? "").ToList() ?? [];

        return new DocumentModel
        {
            Id = efDocument.Id.ToString(),
            Name = efDocument.Name,
            DisplayName = efDocument.DisplayName,
            Description = efDocument.Description,
            CategoryName = efDocument.Category?.DisplayName ?? string.Empty,
            BlobLocation = efDocument.BlobLocation,
            FolderName = efDocument.Register?.Folder?.Name ?? string.Empty,
            Company = efDocument.Company,
            DateOfDocument = efDocument.DateOfDocument,
            DateModified = efDocument.DateModified,
            UploadDate = efDocument.DateCreated,
            Digital = efDocument.Digital,
            PhysicalName = efDocument.PhysicalName,
            Tags = tags,
            RegisterName = efDocument.Register?.Name ?? string.Empty,
            FolderId = efDocument.Register?.Folder?.Id.ToString() ?? string.Empty,
            Unsorted = efDocument.Unsorted
        };
    }
}