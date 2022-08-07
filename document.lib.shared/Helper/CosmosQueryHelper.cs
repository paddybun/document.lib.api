using Microsoft.Azure.Cosmos;

namespace document.lib.shared.Helper
{
    public static class CosmosQueryHelper
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
