using System;
using System.Collections.Generic;

namespace document.lib.api.Models
{
    public class LibDocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public ICollection<DocumentTag> Tags { get; set; }

        public Guid RegisterId{ get; set; }
        public Register Register { get; set; }
    }
}