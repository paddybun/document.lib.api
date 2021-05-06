using Microsoft.Azure.Cosmos.Table;

namespace document.lib.functions.TableEntities
{
    public class DocLibCategory : TableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
