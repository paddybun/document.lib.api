using document.lib.data.entities;
using document.lib.ef;
using document.lib.shared.Helper;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Data;
using document.lib.shared.Models.Update;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class FolderSqlRepository(DocumentLibContext context) : SqlRepositoryBase(context), IFolderRepository<Folder>
{
    public async Task<Folder?> GetFolderAsync(int id)
    {
        return await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Folder?> GetFolderAsync(string name)
    {
        return await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleOrDefaultAsync(x => x.Name == name);
    }

    public async Task<List<Folder>> GetActiveFoldersAsync()
    {
        return await context.Folders
            .Include(x => x.Registers)
            .Where(x => !x.IsFull)
            .ToListAsync();
    }
    
    public async Task<(int, List<Folder>)> GetFolders(int page, int pageSize)
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

    public async Task<List<Folder>> GetAllFoldersAsync()
    {
        var efFolders = await context.Folders
            .Include(x => x.Registers)
            .ToListAsync();
        return efFolders;
    }

    public async Task<Folder> CreateFolderAsync(Folder folder)
    {
        await context.AddAsync(folder);
        await context.SaveChangesAsync();
        return folder;
    }

    public async Task<Folder?> UpdateFolderAsync(FolderUpdateModel folder, string? _ = null)
    {
        if (!PropertyChecker.Values.Any(folder.Id))
            return null;

        var efFolder = new Folder { Id = folder.Id };
        context.Attach(efFolder);
        efFolder.DisplayName = folder.DisplayName;
        efFolder.MaxDocumentsFolder = folder.DocsPerFolder;
        efFolder.MaxDocumentsRegister = folder.DocsPerRegister;
        efFolder.TotalDocuments = folder.TotalDocuments;
        efFolder.IsFull = folder.IsFull;
        await context.SaveChangesAsync();
        return efFolder;
    }

    public async Task<Folder?> AddDocumentToFolderAsync(FolderModel folder, DocumentModel document)
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
            efRegister = new Register
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