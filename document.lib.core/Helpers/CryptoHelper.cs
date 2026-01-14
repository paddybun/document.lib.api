using System.Security.Cryptography;

namespace document.lib.core.Helpers;

public static class CryptoHelper
{
    public static string CreateSha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = global::System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}