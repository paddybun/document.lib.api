using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace document.lib.api.Models
{
    public class LibDocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public ICollection<DocumentTag> Tags { get; set; }
        public Folder Folder { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}