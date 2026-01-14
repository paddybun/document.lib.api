using System.Security.Cryptography;
using System.Text;
using document.lib.core.Models;

namespace document.lib.core.Helpers;

public static class EncryptionHelper
{
    public static EncryptedData Encrypt(string key, string plainText, string? iv = null)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        
        if (iv is { Length: >0 })
            aes.IV = Convert.FromBase64String(iv);
        else
            aes.GenerateIV();

        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            
        var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(plainTextBytes, 0, plainText.Length);
        }

        var newIv = Convert.ToBase64String(aes.IV);
        var toReturn = Convert.ToBase64String(ms.ToArray());
        return new() { EncryptedString = toReturn, Iv = newIv };
    }
    
    public static string Decrypt(string key, EncryptedData data)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key); 
        aes.IV = Convert.FromBase64String(data.Iv);
        
        var encryptedBytes = Convert.FromBase64String(data.EncryptedString);
        
        var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
        {
            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
        }
        var decrypted = Encoding.UTF8.GetString(ms.ToArray());
        return decrypted;
    }
}