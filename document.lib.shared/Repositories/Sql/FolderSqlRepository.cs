using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.Models.ViewModels;
using document.lib.shared.TableEntities;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO;
using Microsoft.EntityFrameworkCore;
using DocLibDocument = document.lib.shared.TableEntities.DocLibDocument;

namespace document.lib.shared.Repositories.Sql;

public class FolderSqlRepository: IFolderRepository
{
    private readonly DocumentLibContext _context;

    public FolderSqlRepository(DocumentLibContext context)
    {
        _context = context;
    }

    public async Task<FolderModel> GetFolderAsync(FolderQueryParameters queryParameters)
    {
        if (!queryParameters.IsValid())
        {
            throw new InvalidQueryParameterException(queryParameters.GetType());
        }

        EfFolder efFolder = null;
        if (queryParameters.Id.HasValue)
        {
            efFolder = await _context.Folders
                .Include(x => x.Registers)
                .SingleOrDefaultAsync(x => x.Id == queryParameters.Id.Value);
        }
        else if (!string.IsNullOrWhiteSpace(queryParameters.Name))
        {
            efFolder = await _context.Folders
                .Include(x => x.Registers)
                .SingleOrDefaultAsync(x => x.Name == queryParameters.Name);
        }
        else if (queryParameters.ActiveFolder.HasValue)
        {
            efFolder = await _context.Folders
                .Include(x => x.Registers)
                .FirstOrDefaultAsync(x => !x.IsFull && (x.Name != "unsorted" || x.Name != "digital"));
        }
        return Map(efFolder);
    }

    public async Task<List<FolderModel>> GetAllFoldersAsync()
    {
        var efFolders = await _context.Folders
            .Include(x => x.Registers)
            .ToListAsync();
        var mapped = efFolders.Select(Map).ToList();
        return mapped;
    }

    public async Task<FolderModel> CreateFolderAsync(FolderModel folder)
    {
        var efRegister = new EfRegister
        {
            Name = "1",
            DisplayName = "1",
            DocumentCount = 0
        };
        var efFolder = new EfFolder
        {
            Name = folder.Name,
            DisplayName = folder.DisplayName,
            CurrentRegister = efRegister,
            IsFull = false,
            MaxDocumentsFolder = folder.DocumentsFolder,
            MaxDocumentsRegister = folder.DocumentsRegister,
            TotalDocuments = 0
        };
        
        await _context.AddAsync(efFolder);
        await _context.SaveChangesAsync();
        return Map(efFolder);
    }

    public async Task<FolderModel> UpdateFolderAsync(FolderModel folder)
    {
        var parsedId = int.Parse(folder.Id);
        var efFolder = await _context.Folders.SingleAsync(x => x.Id == parsedId);
        efFolder.Name = folder.Name;
        efFolder.DisplayName = folder.DisplayName;
        efFolder.MaxDocumentsFolder = folder.DocumentsFolder;
        efFolder.MaxDocumentsRegister = folder.DocumentsRegister;
        efFolder.TotalDocuments = folder.TotalDocuments;
        efFolder.IsFull = folder.IsFull;
        _context.Update(efFolder);
        await _context.SaveChangesAsync();
        return Map(efFolder);
    }

    public async Task AddDocumentToFolderAsync(FolderModel folder, DocumentModel document)
    {
        var efDocument = await _context.Documents.SingleOrDefaultAsync(x => x.Id == int.Parse(document.Id));
        var efFolder = await _context.Folders
            .Include(x => x.Registers)
                .ThenInclude(x => x.Documents)
            .SingleAsync(x => x.Id == int.Parse(document.FolderId));

        var efRegister = efFolder.Registers.SingleOrDefault(x => x.DocumentCount <= efFolder.MaxDocumentsRegister);
        if (efRegister == null)
        {
            var newIx = int.Parse(efFolder.Registers.OrderByDescending(x => x.Name).First().Name);
            efRegister = new EfRegister
            {
                Name = (++newIx).ToString(),
                Documents = new List<ef.Entities.EfDocument> {efDocument},
                DisplayName = "",
                DocumentCount = 1,
                Folder = efFolder
            };
        }
        else
        {
            efRegister.DocumentCount++;
            efRegister.Documents.Add(efDocument);
        }

        efFolder.TotalDocuments++;
        efFolder.IsFull = efFolder.TotalDocuments >= efFolder.MaxDocumentsFolder;
        efFolder.CurrentRegister = efRegister;
        _context.Update(efRegister);
        _context.Update(efFolder);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveDocFromFolderAsync(FolderModel folder, DocumentModel document)
    {
        var efFolder = await _context.Folders
            .Include(x => x.Registers)
                .ThenInclude(x => x.Documents)
            .SingleAsync(x => x.Id == int.Parse(document.FolderId));
        
        var efRegister = efFolder.Registers.Single(x => x.Name == document.RegisterName);
        var newList = new List<EfDocument>(efRegister.Documents);
        newList.RemoveAll(x => x.Id == int.Parse(document.Id));
        efRegister.Documents = newList;
        efRegister.DocumentCount--;
        efFolder.TotalDocuments--;
        _context.Update(efRegister);
        _context.Update(efFolder);
        await _context.SaveChangesAsync();
    }

    private static FolderModel Map(EfFolder efFolder)
    {
        if (efFolder == null) return null;
        return new FolderModel
        {
            Name = efFolder.Name,
            DisplayName = efFolder.DisplayName,
            CreatedAt = efFolder.DateCreated,
            CurrentRegisterName = efFolder.CurrentRegister.Name,
            DocumentsFolder = efFolder.MaxDocumentsFolder,
            DocumentsRegister = efFolder.MaxDocumentsRegister,
            Id = efFolder.Id.ToString(),
            IsFull = efFolder.IsFull,
            Registers = efFolder.Registers.Select(Map).ToList()
        };
    }

    private static RegisterModel Map(EfRegister efRegister)
    {
        if (efRegister == null) return null;
        return new RegisterModel
        {
            Name = efRegister.Name,
            DisplayName = efRegister.DisplayName,
            DocumentCount = efRegister.DocumentCount,
            Id = efRegister.Id.ToString()
        };
    }
}