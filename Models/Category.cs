using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace document.lib.api.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }
}
