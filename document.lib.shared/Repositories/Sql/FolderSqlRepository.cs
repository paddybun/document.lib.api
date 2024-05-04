using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Helper;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using document.lib.shared.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class FolderSqlRepository(DocumentLibContext context) : IFolderRepository<EfFolder>
{
    public async Task<EfFolder?> GetFolderAsync(int id)
    {
        return await context.Folders
            .Include(x => x.Registers)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<EfFolder?> GetFolderAsync(string name)
    {
        return await context.Folders
            .Include(x => x.Registers)
            .SingleOrDefaultAsync(x => x.Name == name);
    }

    public async Task<EfFolder?> GetActiveFolderAsync()
    {
        return await context.Folders
            .Include(x => x.Registers)
            .FirstOrDefaultAsync(x => !x.IsFull);
    }
    
    
    public async Task<(int, List<EfFolder>)> GetFolders(int page, int pageSize)
    {
        var folders = await context.Folders
            .OrderBy(x => x.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var countFolders = context.Folders.Count();
        var mapped = folders.ToList();

        return (countFolders, mapped);
    }

    public async Task<List<EfFolder>> GetAllFoldersAsync()
    {
        var efFolders = await context.Folders
            .Include(x => x.Registers)
            .ToListAsync();
        return efFolders;
    }

    public async Task<EfFolder> CreateFolderAsync(string name, int docsPerRegister = 10, int docsPerFolder = 150, string? displayName = null)
    {
        var regName = 1.ToString();
        var efRegister = new EfRegister
        {
            Name = regName,
            DisplayName = regName
        };

        var efFolder = new EfFolder
        {
            Name = name,
            DisplayName = displayName,
            IsFull = false,
            MaxDocumentsFolder = docsPerFolder,
            MaxDocumentsRegister = docsPerRegister,
            TotalDocuments = 0,
            Registers = [efRegister]
        };

        await context.AddAsync(efFolder);
        await context.SaveChangesAsync();
        return efFolder;
    }

    public async Task<EfFolder?> UpdateFolderAsync(FolderUpdateModel folder, string? _ = null)
    {
        if (!PropertyChecker.Values.Any(folder.Id))
            return null;

        var efFolder = new EfFolder { Id = folder.Id };
        context.Attach(efFolder);
        efFolder.DisplayName = folder.DisplayName;
        efFolder.MaxDocumentsFolder = folder.DocsPerFolder;
        efFolder.MaxDocumentsRegister = folder.DocsPerRegister;
        efFolder.TotalDocuments = folder.TotalDocuments;
        efFolder.IsFull = folder.IsFull;
        await context.SaveChangesAsync();
        return efFolder;
    }

    public async Task<EfFolder?> AddDocumentToFolderAsync(FolderModel folder, DocumentModel document)
    {
        if (!PropertyChecker.Values.Any(document, x => x.Id) && !PropertyChecker.Values.Any(folder, x => x.Id))
        {
            return null;
        }
        
        var documentId = (int)document.Id!;
        var folderId = (int)folder.Id!;
        
        var efDocument = await context.Documents.SingleAsync(x => x.Id == documentId);
        var efFolder = await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleAsync(x => x.Id == folderId);

        var efRegister = efFolder.Registers.SingleOrDefault(x => x.DocumentCount <= efFolder.MaxDocumentsRegister);
        if (efRegister == null)
        {
            var lastIx = int.Parse(efFolder.Registers.OrderByDescending(x => x.Name).First().Name);
            efRegister = new EfRegister
            {
                Name = (++lastIx).ToString(),
                Documents = [efDocument],
                DisplayName = lastIx.ToString(),
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
        context.Update(efRegister);
        context.Update(efFolder);
        await context.SaveChangesAsync();
        
        return efFolder;
    }

    public async Task RemoveDocFromFolderAsync(FolderModel folder, DocumentModel document)
    {
        if (!PropertyChecker.Values.Any(document.Id) && !PropertyChecker.Values.Any(folder, x => x.Id))
        {
            return;
        }
        
        var efFolder = await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleAsync(x => x.Id == (int)folder.Id!);
        efFolder.TotalDocuments--;

        var efRegister = efFolder.Registers.Single(x => x.Name == document.RegisterName);
        efRegister.Documents.RemoveAll(x => x.Id == (int)document.Id!);
        efRegister.DocumentCount--;
        context.Update(efRegister);
        context.Update(efFolder);
        
        await context.SaveChangesAsync();
    }
}