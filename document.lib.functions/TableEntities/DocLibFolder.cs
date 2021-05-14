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
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("currentRegister")]
        public string CurrentRegister { get; set; }
        [JsonProperty("registers")]
        public Dictionary<string, int> Registers { get; set; } = new Dictionary<string, int>();
        [JsonProperty("totalDocuments")]
        public int TotalDocuments { get; set; }
        [JsonProperty("documentsPerRegister")]
        public int DocumentsPerRegister { get; set; } = 10;
        [JsonProperty("documentsPerFolder")]
        public int DocumentsPerFolder { get; set; } = 310;
        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonProperty("isFull")]
        public bool IsFull { get; set; }
    }
}
