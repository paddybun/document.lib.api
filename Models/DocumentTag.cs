using System;

namespace document.lib.api.Models
{
    public class DocumentTag
    {
        public Guid Id { get; set; }
        public Guid LibDocumentId { get; set; }
        public LibDocument LibDocument { get; set; }
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}