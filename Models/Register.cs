using System;
using System.Collections.Generic;

namespace document.lib.api.Models
{
    public class Register
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int DocumentCount { get; set; }
        public ICollection<LibDocument> Documents { get; set; }
        public Guid FolderId { get; set; }
        public Folder Folder { get; set; }
    }
}