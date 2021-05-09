using Newtonsoft.Json;

namespace document.lib.functions.TableEntities
{
    public class DocLibTag
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
