using Newtonsoft.Json;

namespace document.lib.functions.TableEntities
{
    public class DocLibCategory
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
