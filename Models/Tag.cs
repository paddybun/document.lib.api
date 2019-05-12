using System;
using System.Collections.Generic;

namespace document.lib.api.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public ICollection<DocumentTag> Documents { get; set; }
    }
}
