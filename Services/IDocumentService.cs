using System.IO;
using System.Threading.Tasks;

namespace document.lib.api.Services
{
    public interface IDocumentService
    {
        Task UploadDocumentAsync(string blobname, byte[] buffer);
    }
}