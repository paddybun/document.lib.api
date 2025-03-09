using document.lib.ef.Entities;

namespace document.lib.shared.Cqrs.Interfaces;

public interface IFolderQuery
{
    Task<EfFolder?> ExecuteAsync(string folderName);
}