using System;
using System.IO;
using System.Threading.Tasks;
using document.lib.api.Models;

namespace document.lib.api.Services
{
    public interface IDocumentService
    {
        Task UploadDocumentAsync(string filename, Register register, byte[] buffer);
        Task<LibDocument> CreateDocumentAsync(string name, Guid categoryId, Guid folderId, DateTimeOffset date, Guid[] tagIds);
        Task<LibDocument> UpdateDocumentAsync(Guid docId, string name, Guid categoryId, Guid folderId, DateTimeOffset date, Guid[] tagIds);
    }
}