using document.lib.shared.Models.Interfaces;

namespace document.lib.shared.Models.Models;

public class FolderModel : IFolderModel, IFolderModelExtensions
{
    // IFolderModel
    public object? Id { get; set; }
    public string Name { get; set; } =  null!;
    public string? DisplayName { get; set; } = null!;
    public RegisterModel? CurrentRegister { get; set; }
    public string? CurrentRegisterName { get; set; }
    public List<RegisterModel> Registers { get; set; } = [];

    // TODO: Needs to be a calculated field
    public int TotalDocuments { get; set; }
    public int DocumentsRegister { get; set; }
    public int DocumentsFolder { get; set; }
    public bool IsFull { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // IFolderModelExtensions
    public bool IsActive { get; set; }
    public int? GetLastRegisterNumber()
    {
        var lastRegister = Registers?.MaxBy(x => x.Name);
        if (int.TryParse(lastRegister?.Name, out var parsedNumber))
        {
            return parsedNumber;
        }

        return null;
    }

    public static FolderModel New()
    {
        return new FolderModel
        {
            Id = null,
            Name = Guid.NewGuid().ToString(),
            DisplayName = null,
            CurrentRegister = null,
            CurrentRegisterName = null,
            Registers = [],
            TotalDocuments = 0,
            DocumentsRegister = 0,
            DocumentsFolder = 0,
            IsFull = false,
            CreatedAt = DateTimeOffset.Now,
            IsActive = false
        };
    }
}