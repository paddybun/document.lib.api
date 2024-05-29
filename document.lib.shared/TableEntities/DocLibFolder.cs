using Newtonsoft.Json;

namespace document.lib.shared.TableEntities
{
    public class DocLibFolder
    {
        [JsonProperty("id")] public string Id { get; set; } = null!;
        [JsonProperty("name")] public string Name { get; set; } = null!;
        [JsonProperty("displayName")] public string? DisplayName { get; set; }
        [JsonProperty("currentRegister")] public string CurrentRegister { get; set; }
        [JsonProperty("registers")] public Dictionary<string, int> Registers { get; set; } = new Dictionary<string, int>();
        [JsonProperty("totalDocuments")] public int TotalDocuments { get; set; }
        [JsonProperty("documentsPerRegister")] public int DocumentsPerRegister { get; set; } = 10;
        [JsonProperty("documentsPerFolder")] public int DocumentsPerFolder { get; set; } = 200;
        [JsonProperty("createdAt")] public DateTimeOffset CreatedAt { get; set; }
        [JsonProperty("isFull")] public bool IsFull { get; set; }

        public string AddDocument()
        {
            // Update folder, will always add a document. Even if the folder is full.
            TotalDocuments++;
            if (TotalDocuments >= DocumentsPerFolder)
            {
                IsFull = true;
            }

            // Update register
            if (Registers[CurrentRegister] >= DocumentsPerRegister)
            {
                if (int.TryParse(CurrentRegister, out int regIx))
                {
                    var ix = (++regIx).ToString();
                    Registers.Add(ix, 1);
                    CurrentRegister = ix;
                }
            }
            else
            {
                Registers[CurrentRegister]++;
                
            }
            return CurrentRegister;
        }

        public void RemoveDocument(string register)
        {
            if (Registers.ContainsKey(register))
            {
                TotalDocuments--;
                Registers[register]--;
            }
        }
    }
}
