﻿using document.lib.shared.Models.Data;

namespace document.lib.shared.Interfaces.Models;

public interface IFolderModel
{
    object? Id { get; set; }
    string Name { get; set; }
    string? DisplayName { get; set; }
    RegisterModel? CurrentRegister { get; set; }
    string? CurrentRegisterName { get; set; }
    List<RegisterModel> Registers { get; set; }
    int TotalDocuments { get; set; }
    int DocumentsRegister { get; set; }
    int DocumentsFolder { get; set; }
    bool IsFull { get; set; }
    DateTimeOffset CreatedAt { get; set; }
}