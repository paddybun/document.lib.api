﻿using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Exceptions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.QueryDtos;
using document.lib.shared.TableEntities;
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

    public async Task<DocLibFolder> GetFolderAsync(FolderQueryParameters queryParameters)
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

    public async Task<List<DocLibFolder>> GetAllFoldersAsync()
    {
        var efFolders = await _context.Folders.ToListAsync();
        var mapped = efFolders.Select(Map).ToList();
        return mapped;
    }

    public async Task<DocLibFolder> CreateFolderAsync(DocLibFolder folder)
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
            MaxDocumentsFolder = folder.DocumentsPerFolder,
            MaxDocumentsRegister = folder.DocumentsPerRegister,
            TotalDocuments = 0
        };
        
        await _context.AddAsync(efFolder);
        await _context.SaveChangesAsync();
        return Map(efFolder);
    }

    public async Task UpdateFolderAsync(DocLibFolder folder)
    {
        var parsedId = int.Parse(folder.Id);
        var efFolder = await _context.Folders.SingleAsync(x => x.Id == parsedId);
        efFolder.Name = folder.Name;
        efFolder.DisplayName = folder.DisplayName;
        efFolder.MaxDocumentsFolder = folder.DocumentsPerFolder;
        efFolder.MaxDocumentsRegister = folder.DocumentsPerRegister;
        efFolder.TotalDocuments = folder.TotalDocuments;
        efFolder.IsFull = folder.IsFull;
        _context.Update(efFolder);
        await _context.SaveChangesAsync();
    }

    public async Task AddDocumentToFolderAsync(DocLibFolder folder, DocLibDocument document)
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

    public async Task RemoveDocFromFolderAsync(DocLibFolder folder, DocLibDocument document)
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

    private DocLibFolder Map(EfFolder efFolder)
    {
        if (efFolder == null) return null;
        return new DocLibFolder
        {
            Name = efFolder.Name,
            DisplayName = efFolder.DisplayName,
            CreatedAt = efFolder.DateCreated,
            CurrentRegister = efFolder.CurrentRegister.Name,
            DocumentsPerFolder = efFolder.MaxDocumentsFolder,
            DocumentsPerRegister = efFolder.MaxDocumentsRegister,
            Id = efFolder.Id.ToString(),
            IsFull = efFolder.IsFull,
            Registers = efFolder.Registers.ToDictionary(x => x.Name, x => x.DocumentCount)
        };
    }
}