using Newtonsoft.Json;

namespace document.lib.shared.TableEntities
{
    public class DocLibCategory
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
