namespace document.lib.shared.Interfaces.Models;

// Interface for FolderModel

public interface IFolderModelExtensions
{
    bool IsActive { get; set; }
    int? GetLastRegisterNumber();
}