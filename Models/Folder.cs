using System;
using System.Collections.Generic;

namespace document.lib.api.Models
{
    public class Folder
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<LibDocument> Documents { get; set; }
    }
}