using Newtonsoft.Json;

namespace document.lib.functions.TableEntities
{
    public class DocLibCategory
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
