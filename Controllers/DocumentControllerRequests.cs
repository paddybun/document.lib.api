using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace document.lib.api.Controllers
{
    public partial class DocumentController
    {
        public class PutDocumentResponse : GetDocumentResponse { }

        public class GetDocumentResponse
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string[] Tags { get; set; }
            public string Category { get; set; }
            public string Folder { get; set; }
        }

        public class PutDocumentRequest
        {
            public Guid Folder { get; set; }

            public Guid Category { get; set; }

            public string Name { get; set; }

            public Guid[] Tags { get; set; }

            public DateTime Date { get; set; }

            public IFormFile File { get; set; }
        }
    }
}