using System;
using System.IO;
using System.Threading.Tasks;
using document.lib.api.Models;

namespace document.lib.api.Services
{
    public interface IDocumentService
    {
        Task UploadDocumentAsync(string blobname, byte[] buffer);
        Task<LibDocument> SaveDocumentAsync(string name, Guid categoryId, Guid folderId, Guid[] tagIds);
    }
}