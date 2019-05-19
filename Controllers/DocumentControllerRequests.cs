using System;
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
            [JsonProperty("folder")]
            public Guid FolderId { get; set; }

            [JsonProperty("category")]
            public Guid CategoryId { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("tags")]
            public Guid[] Tags { get; set; }

        }
    }
}