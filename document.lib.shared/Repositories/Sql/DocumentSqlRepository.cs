using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using document.lib.shared.Models.QueryDtos;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class DocumentSqlRepository(DocumentLibContext context) : IDocumentRepository
{
    public async Task<DocumentModel> CreateDocumentAsync(DocumentModel document)
    {
        var registerName = document.Digital ? "digital" : document.RegisterName;
        var register = await context
            .Registers
            .SingleOrDefaultAsync(x => x.Name == registerName);
        var category = await context
            .Categories
            .SingleOrDefaultAsync(x => x.Name == document.CategoryName);
        var tags = await context
            .Tags
            .Where(x => document.Tags.Contains(x.Name))
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

    public async Task<DocumentModel> GetDocumentAsync(DocumentQueryParameters queryParameters)
    {
        if (queryParameters == null) throw new ArgumentNullException(nameof(queryParameters));
        if (!queryParameters.IsValid()) throw new InvalidQueryParameterException(queryParameters.GetType());

        EfDocument efDocument;
        if (queryParameters.Id.HasValue)
        {
            efDocument = await context
                .Documents
                .Include(x => x.Register)
                .ThenInclude(x => x.Folder)
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Id == queryParameters.Id.Value);
        }
        else
        {
            efDocument = await context
                .Documents
                .Include(x => x.Register)
                .ThenInclude(x => x.Folder)
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Name == queryParameters.Name);
        }
        return Map(efDocument);
    }

    public async Task<List<DocumentModel>> GetAllDocumentsAsync()
    {
        var efDocuments = await context
            .Documents
            .Include(x => x.Tags)
            .ToListAsync();
        return efDocuments.Select(Map).ToList();
    }

    public async Task<List<DocumentModel>> GetDocumentsPagedAsync(int lastId = 0, int count = 20)
    {
        var efDocuments = await context
            .Documents
            .Include(x => x.Tags)
            .OrderBy(x => x.Id)
            .Where(x => x.Id > lastId)
            .Take(count)                                       
            .ToListAsync();
        return efDocuments.Select(Map).ToList();
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
            .Where(x => x.Register.Folder.Name == folderName)
            .Skip(page).Take(count)
            .ToListAsync();

        var mapped = efDocuments.Select(Map).ToList();
        return mapped;
    }

    public async Task<DocumentModel> UpdateDocumentAsync(DocumentModel document, CategoryModel category = null, FolderModel folder = null,
        TagModel[] tags = null)
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
            var efFolder = await context
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
            FolderName = efDocument.Register.Folder?.Name ?? string.Empty,
            Company = efDocument.Company,
            DateOfDocument = efDocument.DateOfDocument,
            DateModified = efDocument.DateModified,
            UploadDate = efDocument.DateCreated,
            Digital = efDocument.Digital,
            PhysicalName = efDocument.PhysicalName,
            Tags = tags,
            RegisterName = efDocument.Register.Name,
            FolderId = efDocument.Register.Folder?.Id.ToString() ?? string.Empty,
            Unsorted = efDocument.Unsorted
        };
    }
}