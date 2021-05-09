using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace document.lib.functions.Helper
{
    public static class QueryHelper
    {
        public static async Task<List<T>> ExecuteQueryAsync<T>(QueryDefinition query, Container container)
        {
            var resultSet = new List<T>();
            var iterator = container.GetItemQueryIterator<T>(query);
            foreach (var item in await iterator.ReadNextAsync())
            {
                resultSet.Add(item);
            }

            return resultSet;
        }
    }
}
