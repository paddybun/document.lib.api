using System.Security.AccessControl;
using document.lib.shared.Enums;

namespace document.lib.shared.Models;

public class AppConfiguration
{
    public string AllowedHosts { get; set; }

    public string CosmosDbConnection { get; set; }

    public string BlobServiceConnectionString { get; set; }

    public string BlobContainer { get; set; }

    public DatabaseProvider DatabaseProvider { get; set; }
}