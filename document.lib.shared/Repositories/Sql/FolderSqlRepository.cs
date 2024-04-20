using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.ef.Helpers;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class FolderSqlRepository(DocumentLibContext context) : IFolderRepository
{
    public async Task<(int, FolderModel[])> GetFolders(int page, int pageSize)
    {
        var folders = await context.Folders
            .OrderBy(x => x.Id)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var countFolders = context.Folders.Count();
        var mapped = folders.Select(MapShallow).ToArray();

        return (countFolders, mapped);
    }

    public async Task<FolderModel?> GetFolderAsync(FolderModel folderModel)
    {
        EfFolder? efFolder = null;
        if (!string.IsNullOrWhiteSpace(folderModel.Id))
        {
            // Check if the id is a valid integer, if not a different id was used (e.g. cosmos)
            if (!int.TryParse(folderModel.Id, out var parsedId))
            {
                return null;
            }

            efFolder = await context.Folders
                .Include(x => x.Registers)
                .SingleOrDefaultAsync(x => x.Id == parsedId);
        }
        else if (!string.IsNullOrWhiteSpace(folderModel.Name))
        {
            efFolder = await context.Folders
                .Include(x => x.Registers)
                .SingleOrDefaultAsync(x => x.Name == folderModel.Name);
        }
        else if (folderModel.IsActive)
        {
            efFolder = await context.Folders
                .Include(x => x.Registers)
                .FirstOrDefaultAsync(x => !x.IsFull && (x.Name != "unsorted" || x.Name != "digital"));
        }

        // Assign current register, if all registers are full, then current register will be null and must be created via the service
        efFolder = FolderEntityHelpers.AssignCurrentRegister(efFolder);

        return efFolder == null ? null : Map(efFolder);
    }

    public async Task<List<FolderModel>> GetAllFoldersAsync()
    {
        var efFolders = await context.Folders
            .Include(x => x.Registers)
            .ToListAsync();
        var mapped = efFolders.Select(Map).ToList();
        return mapped;
    }

    public async Task<FolderModel> CreateFolderAsync(FolderModel folder)
    {
        var regName = 1.ToString();
        var efRegister = new EfRegister()
        {
            Name = regName,
            DisplayName = regName
        };

        var efFolder = new EfFolder
        {
            Name = folder.Name,
            DisplayName = folder.DisplayName,
            IsFull = false,
            MaxDocumentsFolder = folder.DocumentsFolder,
            MaxDocumentsRegister = folder.DocumentsRegister,
            TotalDocuments = 0,
            Registers = [efRegister],
            CurrentRegister = efRegister
        };

        await context.AddAsync(efFolder);
        await context.SaveChangesAsync();

        return Map(efFolder);
    }

    public async Task<FolderModel?> UpdateFolderAsync(FolderModel folder)
    {
        if (string.IsNullOrWhiteSpace(folder.Id)) return null;

        var parsedId = int.Parse(folder.Id);
        var efFolder = await context.Folders.SingleAsync(x => x.Id == parsedId);
        efFolder.DisplayName = folder.DisplayName;
        efFolder.MaxDocumentsFolder = folder.DocumentsFolder;
        efFolder.MaxDocumentsRegister = folder.DocumentsRegister;
        efFolder.TotalDocuments = folder.TotalDocuments;
        efFolder.IsFull = folder.IsFull;
        context.Update(efFolder);
        await context.SaveChangesAsync();

        return Map(efFolder);
    }

    public async Task AddDocumentToFolderAsync(FolderModel folder, DocumentModel document)
    {
        var efDocument = await context.Documents.SingleAsync(x => x.Id == int.Parse(document.Id));
        
        int.TryParse(document.FolderId, out var id);

        var efFolder = await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleAsync(x => x.Id == id);

        var efRegister = efFolder.Registers.SingleOrDefault(x => x.DocumentCount <= efFolder.MaxDocumentsRegister);
        if (efRegister == null)
        {
            var newIx = int.Parse(efFolder.Registers.OrderByDescending(x => x.Name).First().Name);
            efRegister = new EfRegister
            {
                Name = (++newIx).ToString(),
                Documents = [efDocument],
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
        context.Update(efRegister);
        context.Update(efFolder);
        await context.SaveChangesAsync();
    }

    public async Task RemoveDocFromFolderAsync(FolderModel folder, DocumentModel document)
    {
        var efFolder = await context.Folders
            .Include(x => x.Registers)
            .ThenInclude(x => x.Documents)
            .SingleAsync(x => x.Id == int.Parse(document.FolderId));
        
        var efRegister = efFolder.Registers.Single(x => x.Name == document.RegisterName);
        var newList = new List<EfDocument>(efRegister.Documents);
        newList.RemoveAll(x => x.Id == int.Parse(document.Id));
        efRegister.Documents = newList;
        efRegister.DocumentCount--;
        efFolder.TotalDocuments--;
        context.Update(efRegister);
        context.Update(efFolder);
        await context.SaveChangesAsync();
    }

    private static FolderModel MapShallow(EfFolder efFolder)
    {
        return new FolderModel
        {
            Id = efFolder.Id.ToString(),
            Name = efFolder.Name,
            DisplayName = efFolder.DisplayName,
            CreatedAt = efFolder.DateCreated,
            IsActive = false,
            DocumentsFolder = efFolder.MaxDocumentsFolder,
            DocumentsRegister = efFolder.MaxDocumentsRegister,
            IsFull = efFolder.IsFull,
            TotalDocuments = efFolder.TotalDocuments
        };
    }

    private static FolderModel Map(EfFolder efFolder)
    {
        return new FolderModel
        {
            Name = efFolder.Name,
            DisplayName = efFolder.DisplayName,
            CreatedAt = efFolder.DateCreated,
            IsActive = false,
            CurrentRegisterName = efFolder.CurrentRegister?.Name ?? string.Empty,
            DocumentsFolder = efFolder.MaxDocumentsFolder,
            DocumentsRegister = efFolder.MaxDocumentsRegister,
            Id = efFolder.Id.ToString(),
            IsFull = efFolder.IsFull,
            Registers = efFolder
                            .Registers?.Select(x => Map(x, efFolder))
                            .ToList() ?? [],
            TotalDocuments = efFolder.TotalDocuments,
            CurrentRegister = Map(efFolder.CurrentRegister!, efFolder)
        };
    }

    private static RegisterModel Map(EfRegister efRegister, EfFolder efFolder)
    {
        return new RegisterModel
        {
            Name = efRegister.Name,
            DisplayName = efRegister.DisplayName,
            DocumentCount = efRegister.DocumentCount,
            Id = efRegister.Id.ToString(),
            FolderId = efFolder.Id.ToString(),
            FolderName = efFolder.Name
        };
    }
}