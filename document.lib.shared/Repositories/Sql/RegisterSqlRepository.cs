using document.lib.ef;
using document.lib.ef.Entities;
using document.lib.shared.Interfaces;
using document.lib.shared.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace document.lib.shared.Repositories.Sql;

public sealed class RegisterSqlRepository(DocumentLibContext context) : IRegisterRepository
{
    public Task<RegisterModel> GetRegistersAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<RegisterModel> CreateRegistersAsync(RegisterModel model)
    {
        var efRegister = new EfRegister
        {
            Name = model.Name,
            DisplayName = model.DisplayName,
            DocumentCount = model.DocumentCount
        };
        
        var folder = await context
            .Folders
            .SingleOrDefaultAsync(x => x.Name == model.FolderName);
        if (folder != null) efRegister.Folder = folder;

        context.Registers.Add(efRegister);
        await context.SaveChangesAsync();
        return Map(efRegister);
    }

    private RegisterModel Map(EfRegister efRegister)
    {
        return new RegisterModel
        {
            Id = efRegister.Id.ToString(),
            Name = efRegister.Name,
            DisplayName = efRegister.DisplayName,
            FolderId = efRegister.Folder?.Id.ToString() ?? string.Empty,
            FolderName = efRegister.Folder?.Name ?? string.Empty,
            DocumentCount = efRegister.DocumentCount
        };
    }
}