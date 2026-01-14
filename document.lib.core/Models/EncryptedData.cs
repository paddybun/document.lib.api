namespace document.lib.core.Models;

public class EncryptedData
{
    public required string EncryptedString { get; set; }
    public required string Iv { get; set; }
}