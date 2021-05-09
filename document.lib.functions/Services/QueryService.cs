using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using document.lib.functions.Constants;
using document.lib.functions.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;

namespace document.lib.functions.Services
{
    public class QueryService
    {
        private CloudTableClient _tbc;
        public QueryService()
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsDocumentLibStorage");
            var csa = CloudStorageAccount.Parse(connectionString);
            _tbc = csa.CreateCloudTableClient(new TableClientConfiguration());
        }

        public IEnumerable<DocLibDocument> ExecuteQuery(DocumentQuery query)
        {
            var table = _tbc.GetTableReference(TableNames.Doclib);
            var expressions = new List<Expression<Func<DocLibDocument, bool>>>();
            if (query.Unsorted)
            {
                var param = Expression.Parameter(typeof(DocLibDocument), "x");
                var partKeyProp = Expression.Property(param, "PartitionKey");
                var partKeyConst = Expression.Constant("unsorted", typeof(string));
                var partKeyExpr = Expression.Equal(partKeyProp, partKeyConst);

                var unsortedProp = Expression.Property(param, "Unsorted");
                var unsortedConst = Expression.Constant(true, typeof(bool));
                var unsortedExpr = Expression.Equal(unsortedProp, unsortedConst);

                var body = Expression.And(partKeyExpr, unsortedExpr);
                var lambda = Expression.Lambda<Func<DocLibDocument, bool>>(body, new[] {param});

                var qq = table.CreateQuery<DocLibDocument>().AsQueryable().Where(lambda);
                return table.ExecuteQuery(qq.AsTableQuery()).ToList();
            }

            expressions.Add(x => x.PartitionKey == "document");
            expressions.Add(x => x.Unsorted == false);

            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                expressions.Add(x => x.Category == query.Category);
            }

            if (!string.IsNullOrWhiteSpace(query.Description))
            {
                expressions.Add(x => x.Description == query.Description);
            }

            if (!string.IsNullOrWhiteSpace(query.Company))
            {
                expressions.Add(x => x.Company == query.Company);
            }

            if (!string.IsNullOrWhiteSpace(query.DisplayName))
            {
                expressions.Add(x => x.DisplayName == query.DisplayName);
            }

            if (!string.IsNullOrWhiteSpace(query.PhysicalDocumentName))
            {
                expressions.Add(x => x.PhysicalName.Contains(query.PhysicalDocumentName));
            }

            if (query.Tags?.Length > 0)
            {
                expressions.Add(x => x.Tags.Split('|', StringSplitOptions.RemoveEmptyEntries).Any(y => query.Tags.Contains(y, StringComparer.InvariantCultureIgnoreCase)));
            }

            var dbQuery = table.CreateQuery<DocLibDocument>().AsQueryable();
            foreach (var expression in expressions)
            {
                dbQuery = dbQuery.Where(expression);
            }

            var res = table.ExecuteQuery(dbQuery.AsTableQuery());
            return res;
        }
    }
    public class DocumentQuery
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string DisplayName { get; set; }
        public string[] Tags { get; set; }
        public string PhysicalDocumentName { get; set; }
        public DateTimeOffset? UploadDateFrom { get; set; }
        public DateTimeOffset? UploadDateTo { get; set; }
        public DateTimeOffset? DocumentDateFrom { get; set; }
        public DateTimeOffset? DocumentDateTo { get; set; }
        public bool Unsorted { get; set; }
    }
}
