﻿using document.lib.ef.Entities;
using document.lib.shared.Models.Data;
using document.lib.shared.Models.Update;

namespace document.lib.shared.Interfaces;

public interface IFolderRepository<T>: IRepository
{
    Task<T?> GetFolderAsync(int id);
    Task<T?> GetFolderAsync(string name);
    Task<List<T>> GetActiveFoldersAsync();
    Task<List<T>> GetAllFoldersAsync();
    Task<T> CreateFolderAsync(T folder);
    Task<T?> UpdateFolderAsync(FolderUpdateModel updateModel, string? name = null);
    Task<T?> AddDocumentToFolderAsync(FolderModel folder, DocumentModel document);
    Task RemoveDocFromFolderAsync(FolderModel folder, DocumentModel document);
    Task<(int, List<T>)> GetFolders(int page, int pageSize);
}