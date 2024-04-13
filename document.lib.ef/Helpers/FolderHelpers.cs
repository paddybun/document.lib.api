using document.lib.ef.Entities;

namespace document.lib.ef.Helpers;

public static class FolderHelpers
{
    public static EfFolder? AssignCurrentRegister(EfFolder? folder)
    {
        if (!(folder?.Registers?.Count > 0)) return folder;

        var register = folder.Registers.SingleOrDefault(x => x.DocumentCount <= folder.MaxDocumentsRegister);
        folder.CurrentRegister = register;

        return folder;
    }
}