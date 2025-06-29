
namespace document.lib.core;

public class SharedConfig
{
    public string? AllowedHosts { get; init; }
    public string? CosmosDbConnection { get; init; }
    public string? BlobServiceConnectionString { get; init; }
    public string? BlobContainer { get; init; }
    public string? DbConnectionString { get; init; }
    public string? StorageAccount { get; init; }
}