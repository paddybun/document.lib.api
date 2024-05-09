using document.lib.ef.Entities;
using document.lib.shared.Enums;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;

namespace document.lib.shared.Services;

public class FolderSqlService(IFolderRepository<EfFolder> folderRepository)
    : IFolderService
{
    public async Task<ITypedServiceResult<FolderModel?>> GetFolderAsync(int id)
    {
        try
        {
            var folder = await folderRepository.GetFolderAsync(id);
            return folder != null
                ? ServiceResult.Ok(Map(folder))
                : ServiceResult.ErrorDefault<FolderModel>();           
        }
        catch
        {
            return ServiceResult.Error(default(FolderModel));
        }
    }

    public async Task<ITypedServiceResult<FolderModel?>> GetFolderAsync(string name)
    {
        try
        {
            var folder = await folderRepository.GetFolderAsync(name);
            return folder != null
                ? ServiceResult.Ok(Map(folder))
                : ServiceResult.ErrorDefault<FolderModel>();
        }
        catch
        {
            return ServiceResult.ErrorDefault<FolderModel>();
        }
    }

    public async Task<ITypedServiceResult<FolderModel?>> GetActiveFolderAsync(RetrievalOptions options = RetrievalOptions.GetOnly)
    {
        try
        {
            FolderModel folder;
            var folders = await folderRepository.GetActiveFoldersAsync();
            if (folders.Count != 0 && options == RetrievalOptions.CreateIfNotExists)
            {
                var folderCreateResult = await CreateNewFolderAsync();
                folder = folderCreateResult.IsSuccess
                    ? folderCreateResult.Data!
                    : null!;
            }
            else
            {
                folder = Map(folders.First());
            }

            return ServiceResult.Ok(folder);
        }
        catch
        {
            return ServiceResult.ErrorDefault<FolderModel>();
        }
    }

    public async Task<ITypedServiceResult<(int, List<FolderModel>)>> GetFoldersPaged(int page, int pageSize)
    {
        try
        {
            var (count, folders) = await folderRepository.GetFolders(page, pageSize);
            var mapped = folders.Select(MapShallow).ToList();
            return ServiceResult.Ok((count, mapped));
        }
        catch
        {
            return ServiceResult.ErrorDefault<(int, List<FolderModel>)>();
        }
    }

    public async Task<ITypedServiceResult<FolderModel>> CreateNewFolderAsync(int docsPerFolder = 150, int docsPerRegister = 10, string? displayName = null)
    {
        try
        {
            var name = Guid.NewGuid().ToString();
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

            var folder = await folderRepository.CreateFolderAsync(efFolder);
            return ServiceResult.Ok(Map(folder));
        }
        catch
        {
            return ServiceResult.ErrorDefault<FolderModel>();
        }
        
    }


    public async Task<ITypedServiceResult<List<FolderModel>>> GetAllAsync()
    {
        try
        {
            var folders = (await folderRepository.GetAllFoldersAsync())
                .Select(Map)
                .ToList();
            return ServiceResult.Ok(folders);
        }
        catch
        {
            return ServiceResult.ErrorDefault<List<FolderModel>>();
        }
    }

    public async Task<ITypedServiceResult<FolderModel?>> UpdateFolderAsync(FolderModel folder)
    {
        try
        {
            var efFolder = await folderRepository.GetFolderAsync((int)folder.Id!);
            if (efFolder == null) return ServiceResult.ErrorDefault<FolderModel>();
            
            efFolder.MaxDocumentsFolder = folder.DocumentsFolder;
            efFolder.MaxDocumentsRegister = folder.DocumentsRegister;
            efFolder.DisplayName = folder.DisplayName;

            await folderRepository.SaveAsync();

            return ServiceResult.Ok(Map(efFolder));
        }
        catch
        {
            return ServiceResult.ErrorDefault<FolderModel>();
        }
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