using document.lib.data.entities;
using document.lib.shared.Enums;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Data;
using document.lib.shared.Models.Result;

namespace document.lib.shared.Services;

public class FolderSqlService(IFolderRepository<Folder> folderRepository)
    : IFolderService
{
    public async Task<ITypedServiceResult<FolderModel?>> GetFolderAsync(int id)
    {
        try
        {
            var folder = await folderRepository.GetFolderAsync(id);
            return folder != null
                ? ServiceResult.Ok(Map(folder))
                : ServiceResult.DefaultError<FolderModel>();           
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
                : ServiceResult.DefaultError<FolderModel>();
        }
        catch
        {
            return ServiceResult.DefaultError<FolderModel>();
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
            return ServiceResult.DefaultError<FolderModel>();
        }
    }

    public async Task<ITypedServiceResult<PagedResult<FolderModel>>> GetFoldersPaged(int page, int pageSize)
    {
        try
        {
            var (count, folders) = await folderRepository.GetFolders(page, pageSize);
            var mapped = folders.Select(MapShallow).ToList();
            return ServiceResult.Ok(new PagedResult<FolderModel>(mapped, count));
        }
        catch
        {
            return ServiceResult.DefaultError<PagedResult<FolderModel>>();
        }
    }

    public async Task<ITypedServiceResult<FolderModel>> CreateNewFolderAsync(int docsPerFolder = 150, int docsPerRegister = 10, string? displayName = null)
    {
        try
        {
            var name = Guid.NewGuid().ToString();
            var regName = 1.ToString();
            var efRegister = new Register
            {
                Name = regName,
                DisplayName = regName
            };

            var efFolder = new Folder
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
            return ServiceResult.DefaultError<FolderModel>();
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
            return ServiceResult.DefaultError<List<FolderModel>>();
        }
    }

    public async Task<ITypedServiceResult<FolderModel?>> UpdateFolderAsync(FolderModel folder)
    {
        try
        {
            var efFolder = await folderRepository.GetFolderAsync((int)folder.Id!);
            if (efFolder == null) return ServiceResult.DefaultError<FolderModel>();
            
            efFolder.MaxDocumentsFolder = folder.DocumentsFolder;
            efFolder.MaxDocumentsRegister = folder.DocumentsRegister;
            efFolder.DisplayName = folder.DisplayName;

            await folderRepository.SaveAsync();

            return ServiceResult.Ok(Map(efFolder));
        }
        catch
        {
            return ServiceResult.DefaultError<FolderModel>();
        }
    }
    
    private static FolderModel MapShallow(Folder folder)
    {
        return new FolderModel
        {
            Id = folder.Id.ToString(),
            Name = folder.Name,
            DisplayName = folder.DisplayName,
            CreatedAt = folder.DateCreated,
            IsActive = false,
            DocumentsFolder = folder.MaxDocumentsFolder,
            DocumentsRegister = folder.MaxDocumentsRegister,
            IsFull = folder.IsFull,
            TotalDocuments = folder.TotalDocuments
        };
    }

    private static FolderModel Map(Folder folder)
    {
        return new FolderModel
        {
            Name = folder.Name,
            DisplayName = folder.DisplayName,
            CreatedAt = folder.DateCreated,
            IsActive = false,
            CurrentRegisterName = folder.CurrentRegister?.Name ?? string.Empty,
            DocumentsFolder = folder.MaxDocumentsFolder,
            DocumentsRegister = folder.MaxDocumentsRegister,
            Id = folder.Id.ToString(),
            IsFull = folder.IsFull,
            Registers = folder
                .Registers?.Select(x => Map(x, folder))
                .ToList() ?? [],
            TotalDocuments = folder.TotalDocuments,
            CurrentRegister = Map(folder.CurrentRegister!, folder)
        };
    }

    private static RegisterModel Map(Register register, Folder folder)
    {
        return new RegisterModel
        {
            Name = register.Name,
            DisplayName = register.DisplayName,
            DocumentCount = register.DocumentCount,
            Id = register.Id.ToString(),
            FolderId = folder.Id.ToString(),
            FolderName = folder.Name
        };
    }
}