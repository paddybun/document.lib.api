using System;
using System.IO;
using System.Threading.Tasks;
using document.lib.api.Controllers;
using document.lib.api.Dtos;
using document.lib.api.Models;

namespace document.lib.api.Services
{
    public interface IDocumentService
    {
        Task UploadDocumentAsync(string filename, Register register, byte[] buffer);
        Task<LibDocument> CreateDocumentAsync(string blobname, DocumentController.PostDocumentRequest request);
        Task<LibDocument> UpdateDocumentAsync(Guid docId, string name, Guid categoryId, Guid folderId, DateTimeOffset date, string[] tags);
        Task<DocumentDownloadDto> DownloadDocumentAsync(Guid docId);
        Task DeleteDocumentAsync(Guid docId);
    }
}