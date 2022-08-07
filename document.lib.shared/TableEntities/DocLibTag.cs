using Newtonsoft.Json;

namespace document.lib.shared.TableEntities
{
    public class DocLibTag
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
