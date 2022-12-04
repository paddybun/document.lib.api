using System.Security.AccessControl;

namespace document.lib.shared.Models;

public class AppConfiguration
{
    public string AllowedHosts { get; set; }

    public string CosmosDbConnection { get; set; }

    public string BlobContainerConnectionString { get; set; }

    public string BlobContainer { get; set; }
}