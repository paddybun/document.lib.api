using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace document.lib.functions.TableEntities
{
    public class DocLibFolder
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string CurrentRegister { get; set; }
        public Dictionary<string, int> Registers { get; set; } = new Dictionary<string, int>();
        public int TotalDocuments { get; set; }
        public int DocumentsPerRegister { get; set; } = 10;
        public int DocumentsPerFolder { get; set; } = 310;
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsFull { get; set; }
    }
}
