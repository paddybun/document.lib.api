using Newtonsoft.Json;

namespace document.lib.shared.TableEntities
{
    public class DocLibTag
    {
        [JsonProperty("id")]
        public string Id { get; set; } = null!;
        [JsonProperty("name")]
        public string Name { get; set; } = null!;
        [JsonProperty("displayName")]
        public string? DisplayName { get; set; }
    }
}
