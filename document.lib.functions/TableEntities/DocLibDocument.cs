using System;
using Microsoft.Azure.Cosmos.Table;

namespace document.lib.functions.TableEntities
{
    public class DocLibDocument : TableEntity
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string PhysicalName { get; set; } = "";
        public string BlobLocation { get; set; } = "";
        public string Company { get; set; } = "";
        public DateTimeOffset? DateOfDocument { get; set; }
        public DateTimeOffset UploadDate { get; set; }
        public string Description { get; set; } = "";
        public string FolderName { get; set; } = "";
        public string RegisterName { get; set; } = "";
        public string Tags { get; set; }
        public bool Unsorted { get; set; }
        public string Category { get; set; } = "";
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(DisplayName) ||
                string.IsNullOrWhiteSpace(Company) ||
                DateOfDocument == default ||
                string.IsNullOrWhiteSpace(Category) ||
                string.IsNullOrWhiteSpace(Tags))
            {
                throw new Exception("Please make sure the following fields are filled ['DisplayName, Company, DateOfDocument, Category, Tags']");
            }
        }
    }
}
