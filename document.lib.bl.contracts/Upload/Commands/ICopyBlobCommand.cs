namespace document.lib.bl.contracts.Upload.Commands;

public interface ICopyBlobCommand
{
    Task<bool> ExecuteAsync(string sourcePath, string destinationPath);
}
