using Microsoft.Azure.Cosmos.Table;

namespace document.lib.functions.TableEntities
{
    public class DocLibTag: TableEntity
    {
        public string Name { get; set; }
    }
}
